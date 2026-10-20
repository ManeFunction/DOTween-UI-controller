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
            // Autoplay can restart on enable; without it, tweens can be created on Start
            SerializedProperty autoplay = serializedObject.FindProperty("_autoplay");
            root.Require<Toggle>("restartOnEnableToggle")?.ShowWhen(() => autoplay.boolValue, autoplay);
            root.Require<Toggle>("createOnStartToggle")?.ShowWhen(() => !autoplay.boolValue, autoplay);

            // Setup each tween data block
            SetupTweenData(DOTweenUIController.TweenType.MoveX, root, "moveX", "Move X");
            SetupTweenData(DOTweenUIController.TweenType.MoveY, root, "moveY", "Move Y");
            SetupTweenData(DOTweenUIController.TweenType.ScaleX, root, "scaleX", "Scale X");
            SetupTweenData(DOTweenUIController.TweenType.ScaleY, root, "scaleY", "Scale Y");
            SetupTweenData(DOTweenUIController.TweenType.Rotate, root, "rotate", "Rotate Z");
            SetupTweenData(DOTweenUIController.TweenType.Fade, root, "fade", "Fade");
            SetupTweenData(DOTweenUIController.TweenType.Color, root, "color", "Color");

            // Play-mode-only controls: built-in VisualElement.tooltip is suppressed in Play Mode
            SetupEditorButtons(root);
        }

        private void SetupTweenData(DOTweenUIController.TweenType tweenType, VisualElement root,
            string elementName, string label)
        {
            TemplateContainer tweenDataElement = root.Require<TemplateContainer>(elementName);
            FeatureBlock block = tweenDataElement?.Require<FeatureBlock>("featureBlock");
            if (block == null)
                return;

            block.Header = label;
            AddRequiredComponentBox(tweenType, block);

            // The UXML instance binds the block to its tween data
            SerializedProperty data = serializedObject.FindProperty(tweenDataElement.bindingPath);
            if (data == null)
            {
                Debug.LogError($"'{tweenDataElement.bindingPath}' was not found on {nameof(DOTweenUIController)}.");
                return;
            }

            SerializedProperty loopCount = data.FindPropertyRelative("_loopCount");
            SerializedProperty useCurve = data.FindPropertyRelative("_easeData._useCurve");
            tweenDataElement.Require<VisualElement>("loopOptions")?.ShowWhen(() => loopCount.intValue != 0, loopCount);
            tweenDataElement.Require<VisualElement>("curveField")?.ShowWhen(() => useCurve.boolValue, useCurve);
            tweenDataElement.Require<VisualElement>("easeField")?.ShowWhen(() => !useCurve.boolValue, useCurve);
        }

        // Explains that a tween's component is missing, with an "Add" action when it can be added, and locks its
        // toggle. The stored setting is left alone: tweens skip a slot without its component at runtime anyway,
        // and once the component is added the slot works as it was set.
        private void AddRequiredComponentBox(DOTweenUIController.TweenType tweenType, FeatureBlock block)
        {
            Type requiredComponent = DOTweenUIController.GetRequiredComponent(tweenType);
            if (requiredComponent == null || TargetHasComponent(requiredComponent))
                return;

            Toggle isEnableToggle = block.Toggle;
            isEnableToggle.SetEnabled(false);

            string message = $"'{requiredComponent.Name}' is required";
            Type componentToAdd = GetAddableComponentType(requiredComponent);
            VisualElement messageBox = null;
            Action addComponent = componentToAdd == null
                ? null
                : () =>
                {
                    GameObject gameObject = ((DOTweenUIController)target).gameObject;
                    if (gameObject.GetComponent(requiredComponent) != null)
                        return;

                    Undo.AddComponent(gameObject, componentToAdd);
                    messageBox.SetVisible(false);
                    isEnableToggle.SetEnabled(true);
                    isEnableToggle.tooltip = string.Empty;
                };

            messageBox = InfoBoxDrawer.Create(message, InfoBoxType.Warning, "Add", addComponent);
            messageBox.name = "messageBox";
            messageBox.tooltip = isEnableToggle.tooltip = message;

            // Above the toggle, outside the content, so it stays visible while the tween is off.
            block.hierarchy.Insert(0, messageBox);
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
            buttonsRow.SetVisible(showButtons);
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
            Button button = root.Require<Button>(buttonName);
            if (button != null)
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