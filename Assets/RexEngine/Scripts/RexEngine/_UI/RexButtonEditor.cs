using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace RexEngine
{
	#if UNITY_EDITOR
	[CustomEditor(typeof(RexButton))]
	public class RexButtonEditor:Editor 
	{
		protected List<MethodInfo> methods = new List<MethodInfo>();
		protected List<string> methodNames = new List<string>();
		protected MonoBehaviour previousMonoBehavior;

		void OnEnable()
		{
			if(methodNames == null || methodNames.Count < 1)
			{
				SetupMethods();
			}
		}

		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			RexButton rexButton = target as RexButton;

			if(rexButton.interaction == RexButton.Interaction.OpenURL)
			{
				rexButton.url = EditorGUILayout.TextField("URL", rexButton.url);
			}
			else if(rexButton.interaction == RexButton.Interaction.CallFunction)
			{
				rexButton.callFunctionOnScript = EditorGUILayout.ObjectField("Call Function on Script", rexButton.callFunctionOnScript, typeof(MonoBehaviour), true) as MonoBehaviour;

				//EditorGUILayout.ObjectField("Label:", target.myClass, typeof(MyClass), true);

				if(rexButton.callFunctionOnScript != null)
				{
					if(methodNames == null || methodNames.Count < 1 || previousMonoBehavior != rexButton.callFunctionOnScript)
					{
						methods = null;
						methodNames = null;
						SetupMethods();

						if(rexButton.methodIndex > methodNames.Count - 1)
						{
							rexButton.methodIndex = methodNames.Count - 1;
						}
					}

					if(methodNames != null && methodNames.Count > 0)
					{
						rexButton.methodIndex = EditorGUILayout.Popup("Function", rexButton.methodIndex, methodNames.ToArray());
						rexButton.methodName = methodNames[rexButton.methodIndex];
					}
				}

				if(rexButton.callFunctionOnScript != null)
				{
					previousMonoBehavior = rexButton.callFunctionOnScript;
				}
			}

			EditorUtility.SetDirty(target);
		}

		protected void SetupMethods()
		{
			RexButton rexButton = target as RexButton;
			if(rexButton != null && rexButton.interaction == RexButton.Interaction.CallFunction && rexButton.callFunctionOnScript != null)
			{
				if(methods == null)
				{
					methods = new List<MethodInfo>();
				}

				if(methodNames == null)
				{
					methodNames = new List<string>();
				}

				methods.AddRange(rexButton.callFunctionOnScript.GetPublicVoids());  

				foreach(MethodInfo methodInfo in methods)
				{
					methodNames.Add(methodInfo.Name);
				}
			}
		}
	}
	#endif
}

