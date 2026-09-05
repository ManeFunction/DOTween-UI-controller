using System;
using Mane.Unity.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Image = UnityEngine.UI.Image;
using Toggle = UnityEngine.UIElements.Toggle;

namespace Mane.Unity.DOTween.Editor
{
    [CustomEditor(typeof(DOTweenUIController))]
    public class DOTweenUIControllerEditor : ManeEditor
    {
        protected override void BuildInspector(VisualElement root)
        {
            // Setup autoplay toggles
            Toggle restartOnEnableToggle = root.Q<Toggle>("restartOnEnableToggle");
            Toggle autoplayToggle = root.Q<Toggle>("autoplayToggle");
            Toggle createOnStartToggle = root.Q<Toggle>("createOnStartToggle");
            if (restartOnEnableToggle == null || autoplayToggle == null || createOnStartToggle == null)
            {
                Debug.LogError("One or more UI elements for autoplay data not found.");
            }
            else
            {
                autoplayToggle.RegisterValueChangedCallback(_ => SetMainTogglesVisibility());
                SetMainTogglesVisibility();
            }

            // Setup visibility callbacks for each tween data block
            SetupTweenData(DOTweenUIController.TweenType.MoveX, root, "moveX", "Move X", typeof(RectTransform));
            SetupTweenData(DOTweenUIController.TweenType.MoveY, root, "moveY", "Move Y", typeof(RectTransform));
            SetupTweenData(DOTweenUIController.TweenType.ScaleX, root, "scaleX", "Scale X", typeof(RectTransform));
            SetupTweenData(DOTweenUIController.TweenType.ScaleY, root, "scaleY", "Scale Y", typeof(RectTransform));
            SetupTweenData(DOTweenUIController.TweenType.Rotate, root, "rotate", "Rotate Z", typeof(RectTransform));
            SetupTweenData(DOTweenUIController.TweenType.Fade, root, "fade", "Fade", typeof(CanvasGroup));
            SetupTweenData(DOTweenUIController.TweenType.Color, root, "color", "Color", typeof(MaskableGraphic));

            // Play-mode-only controls: built-in VisualElement.tooltip is suppressed in Play Mode
            SetupEditorButtons(root);

            void SetMainTogglesVisibility()
            {
                restartOnEnableToggle.style.display = autoplayToggle.value ? DisplayStyle.Flex : DisplayStyle.None;
                createOnStartToggle.style.display = autoplayToggle.value ? DisplayStyle.None : DisplayStyle.Flex;
            }
        }

        private void SetupTweenData(DOTweenUIController.TweenType tweenType, VisualElement root,
            string elementName, string label, Type requiredComponent = null)
        {
            VisualElement tweenDataElement = root.Q<VisualElement>(elementName);
            if (tweenDataElement == null)
            {
                Debug.LogError($"VisualElement '{elementName}' not found in root.");
                return;
            }

            // Content container
            VisualElement contentContainer = tweenDataElement.Q<VisualElement>("contentContainer");
            if (contentContainer == null)
            {
                Debug.LogError($"Content container 'contentContainer' not found in '{elementName}'.");
                return;
            }

            // Toggle
            Toggle isEnableToggle = tweenDataElement.Q<Toggle>("isEnableToggle");
            if (isEnableToggle == null)
            {
                Debug.LogError($"Toggle 'isEnableToggle' not found in '{elementName}'.");
                return;
            }
            
            // Message box
            VisualElement messageBox = tweenDataElement.Q<VisualElement>("messageBox");
            if (messageBox == null)
            {
                Debug.LogError($"VisualElement 'messageBox' not found in '{elementName}'.");
                return;
            }
            
            isEnableToggle.text = label;
            
            // Required component
            Button addComponentButton = messageBox.Q<Button>("addComponentButton");
            Type componentToAdd = GetAddableComponentType(requiredComponent);
            if (requiredComponent != null && !TargetHasComponent(requiredComponent))
            {
                ((DOTweenUIController)target).SetTweenEnabled(tweenType, false);
                isEnableToggle.SetEnabled(false);
                Label messageLabel = messageBox.Q<Label>("messageBoxLabel");
                messageLabel.text = messageBox.tooltip = isEnableToggle.tooltip =
                    $"'{requiredComponent.Name}' is required";

                if (addComponentButton == null)
                {
                    Debug.LogError($"Button 'addComponentButton' not found in '{elementName}'.");
                }
                else if (componentToAdd == null)
                {
                    addComponentButton.style.display = DisplayStyle.None;
                }
                else
                {
                    addComponentButton.clicked += () =>
                    {
                        GameObject gameObject = ((DOTweenUIController)target).gameObject;
                        if (gameObject.GetComponent(requiredComponent) != null)
                            return;

                        Undo.AddComponent(gameObject, componentToAdd);
                    };
                }
            }
            else
            {
                messageBox.style.display = DisplayStyle.None;
            }

            // Initialize base visibility
            UpdateContentVisibility();

            isEnableToggle.RegisterValueChangedCallback(_ => { UpdateContentVisibility(); });
            
            // Handle looping value
            IntegerField loopField = contentContainer.Q<IntegerField>("loopCountField");
            VisualElement loopOptions = contentContainer.Q<VisualElement>("loopOptions");
            if (loopField == null || loopOptions == null)
            {
                Debug.LogError("One or more UI elements for loop data not found.");
                return;
            }
            
            // Initialize loop type visibility
            UpdateLoopField();
            
            loopField.RegisterValueChangedCallback(_ => { UpdateLoopField(); });

            // Handle useCurveToggle
            Toggle useCurveToggle = contentContainer.Q<Toggle>("useCurveToggle");
            VisualElement curveField = contentContainer.Q<VisualElement>("curveField");
            VisualElement easeField = contentContainer.Q<VisualElement>("easeField");

            if (useCurveToggle == null || curveField == null || easeField == null)
            {
                Debug.LogError("One or more UI elements for ease data not found.");
                return;
            }

            // Initialize easing fields visibility
            UpdateEaseFields();

            useCurveToggle.RegisterValueChangedCallback(_ => { UpdateEaseFields(); });

            return;

            void UpdateContentVisibility()
            {
                bool isEnabled = isEnableToggle.value;
                contentContainer.style.display = isEnabled ? DisplayStyle.Flex : DisplayStyle.None;
            }

            void UpdateLoopField()
            {
                loopOptions.style.display = loopField.value == 0 ? DisplayStyle.None : DisplayStyle.Flex;
            }

            void UpdateEaseFields()
            {
                bool useCurve = useCurveToggle.value;
                curveField.style.display = useCurve ? DisplayStyle.Flex : DisplayStyle.None;
                easeField.style.display = useCurve ? DisplayStyle.None : DisplayStyle.Flex;
            }
        }
            
        private void SetupEditorButtons(VisualElement root)
        {
            VisualElement buttonsRow = root.Q<VisualElement>(className: "buttons-row");
            if (buttonsRow == null)
            {
                Debug.LogError("Buttons row not found.");
                return;
            }

            bool showButtons = Application.isPlaying;
            buttonsRow.style.display = showButtons ? DisplayStyle.Flex : DisplayStyle.None;
            if (!showButtons)
                return;

            InitButton(root, "playButton", () => ((DOTweenUIController)target).Restart());
            InitButton(root, "playBackwardsButton", () => ((DOTweenUIController)target).RestartBackwards());
            InitButton(root, "stopButton", () => ((DOTweenUIController)target).Stop());
            InitButton(root, "rewindButton", () => ((DOTweenUIController)target).Rewind());
            InitButton(root, "recreateButton", () => ((DOTweenUIController)target).UndoAndDispose());

            // Custom tooltips: Unity suppresses VisualElement.tooltip while playing
            Label tip = new() { pickingMode = PickingMode.Ignore };
            tip.AddToClassList("button-tooltip");
            tip.style.display = DisplayStyle.None;
            root.Add(tip);

            IVisualElementScheduledItem showTip = null;
            Button hoveredButton = null;

            tip.RegisterCallback<GeometryChangedEvent>(_ =>
            {
                if (hoveredButton == null || tip.style.display == DisplayStyle.None)
                    return;

                Rect buttonBounds = hoveredButton.worldBound;
                Rect rootBounds = root.worldBound;
                tip.style.left = buttonBounds.center.x - rootBounds.xMin - tip.resolvedStyle.width * .5f;
                tip.style.top = buttonBounds.yMin - rootBounds.yMin - tip.resolvedStyle.height - 2f;
            });

            foreach (Button button in buttonsRow.Query<Button>().ToList())
            {
                button.RegisterCallback<PointerEnterEvent>(_ =>
                {
                    showTip?.Pause();
                    hoveredButton = button;
                    showTip = root.schedule.Execute(() =>
                    {
                        if (hoveredButton == null || string.IsNullOrEmpty(hoveredButton.tooltip))
                            return;

                        tip.text = hoveredButton.tooltip;
                        tip.style.display = DisplayStyle.Flex;
                        tip.BringToFront();
                    }).StartingIn(400);
                });

                button.RegisterCallback<PointerLeaveEvent>(_ =>
                {
                    showTip?.Pause();
                    hoveredButton = null;
                    tip.style.display = DisplayStyle.None;
                });
            }
        }

        private static void InitButton(VisualElement root, string buttonName, Action action)
        {
            Button button = root.Q<Button>(buttonName);
            if (button == null)
            {
                Debug.LogError($"{buttonName} button not found.");
                return;
            }

            button.clicked += action;
        }

        private bool TargetHasComponent(Type componentType) => 
            ((DOTweenUIController)target).GetComponent(componentType) != null;

        private static Type GetAddableComponentType(Type requiredComponent)
        {
            if (requiredComponent == null || !typeof(Component).IsAssignableFrom(requiredComponent))
                return null;
            if (!requiredComponent.IsAbstract)
                return requiredComponent;
            if (requiredComponent == typeof(MaskableGraphic))
                return typeof(Image);

            return null;
        }
    }
}