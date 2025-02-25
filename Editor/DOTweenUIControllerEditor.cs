using System;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using Toggle = UnityEngine.UIElements.Toggle;

namespace ManeFunction.DOTweenExtensions.Editor
{
    [CustomEditor(typeof(DOTweenUIController))]
    public class DOTweenUIControllerEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset xml;
        
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new();
            xml.CloneTree(root);
            
            // Force update the bindings to get the latest values (Unity 2022.2- fix)
            SerializedObject so = new(target);
            root.Bind(so);
            
            // Fixed an issue with different fields layout before and after Unity 2022.2
#if !UNITY_2022_2_OR_NEWER
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Packages/com.manefunction.dotween-ui-controller/Editor/LegacyLayoutFix.uss");
            if (styleSheet != null)
                root.styleSheets.Add(styleSheet);
#endif
            
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
                autoplayToggle.RegisterValueChangedCallback(evt => SetMainTogglesVisibility());
                SetMainTogglesVisibility();
            }

            // Setup visibility callbacks for each tween data block
            SetupTweenData(DOTweenUIController.TweenType.MoveX, root, "moveX", "Move X");
            SetupTweenData(DOTweenUIController.TweenType.MoveY, root, "moveY", "Move Y");
            SetupTweenData(DOTweenUIController.TweenType.ScaleX, root, "scaleX", "Scale X");
            SetupTweenData(DOTweenUIController.TweenType.ScaleY, root, "scaleY", "Scale Y");
            SetupTweenData(DOTweenUIController.TweenType.Rotate, root, "rotate", "Rotate Z");
            
            SetupTweenData(DOTweenUIController.TweenType.Fade, root, "fade", "Fade",
                TargetHasComponent<CanvasGroup>, 
                "GameObject should have a CanvasGroup component to use fade tweening.");
            
            SetupTweenData(DOTweenUIController.TweenType.Color, root, "color", "Color",
                TargetHasComponent<MaskableGraphic>, 
                "GameObject should have any MaskableGraphic component to use color tweening.");

            return root;
            
            
            void SetMainTogglesVisibility()
            {
                restartOnEnableToggle.style.display = autoplayToggle.value ? DisplayStyle.Flex : DisplayStyle.None;
                createOnStartToggle.style.display = autoplayToggle.value ? DisplayStyle.None : DisplayStyle.Flex;
            }
        }

        private void SetupTweenData(DOTweenUIController.TweenType tweenType, VisualElement root,
            string elementName, string label, Func<bool> optionalCondition = null,
            string optionalConditionErrorMessage = "")
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
            
            // Optional condition
            if (optionalCondition != null && !optionalCondition())
            {
                ((DOTweenUIController)target).SetTweenEnabled(tweenType, false);
                isEnableToggle.SetEnabled(false);
                Label messageLabel = messageBox.Q<Label>("messageBoxLabel");
                messageLabel.text = messageBox.tooltip = isEnableToggle.tooltip = optionalConditionErrorMessage;
            }
            else
            {
                messageBox.style.display = DisplayStyle.None;
            }

            // Initialize base visibility
            UpdateContentVisibility();

            isEnableToggle.RegisterValueChangedCallback(evt => { UpdateContentVisibility(); });
            
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
            
            loopField.RegisterValueChangedCallback(evt => { UpdateLoopField(); });

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

            useCurveToggle.RegisterValueChangedCallback(evt => { UpdateEaseFields(); });
            
            // Editor Play buttons
            InitButton("playButton", () => ((DOTweenUIController)target).Restart());
            InitButton("playBackwardsButton", () => ((DOTweenUIController)target).RestartBackwards());
            InitButton("stopButton", () => ((DOTweenUIController)target).Stop());
            InitButton("rewindButton", () => ((DOTweenUIController)target).Rewind());
            InitButton("recreateButton", () => ((DOTweenUIController)target).UndoAndDispose());

            return;
            

            void InitButton(string buttonName, Action action)
            {
                Button button = root.Q<Button>(buttonName);
                if (button == null)
                {
                    Debug.LogError($"{buttonName} button not found.");
                    return;
                }
                
                button.clicked += action;
                button.style.display = Application.isPlaying ? DisplayStyle.Flex : DisplayStyle.None;
            }

            void UpdateContentVisibility()
            {
                bool isEnabled = isEnableToggle.value;
                contentContainer.style.display = isEnabled ? DisplayStyle.Flex : DisplayStyle.None;
            }

            void UpdateLoopField()
            {
                if (loopField.value < -1)
                    loopField.value = -1;
                loopOptions.style.display = loopField.value == 0 ? DisplayStyle.None : DisplayStyle.Flex;
            }

            void UpdateEaseFields()
            {
                bool useCurve = useCurveToggle.value;
                curveField.style.display = useCurve ? DisplayStyle.Flex : DisplayStyle.None;
                easeField.style.display = useCurve ? DisplayStyle.None : DisplayStyle.Flex;
            }
        }
            
        private bool TargetHasComponent<T>() where T : Component => 
            ((DOTweenUIController)target).GetComponent<T>() != null;
    }
}