/* Copyright Sky Tyrannosaur */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RexEngine
{
	public class DialogueManager:MonoBehaviour 
	{
		[System.Serializable]
		public class DialogueSettings
		{
			public float delayBetweenLetters = 0.025f;
			public float textFieldWidth = 130.0f;
		}

		[System.Serializable]
		public class Page
		{
			public string text;
		}

		public DialogueSettings dialogueSettings;
		public TextMesh text;
		public AudioSource audioSource;
		public AudioClip showSound;
		public AudioClip hideSound;
		public GameObject advanceIcon;

		protected bool willStopTimeOnShow = false;
		protected bool isCloseEnabled = false;
		protected bool willSkipToEnd = false;
		protected bool isDialogueActive = false;

		protected PageInfo pageInfo = new PageInfo();
		protected List<Page> pages;

		private string currentlyDisplayedText;
		private string[] wordsInText;

		public delegate void OnDialogueComplete();
		public OnDialogueComplete onDialogueComplete;

		public class PageInfo
		{
			public int total = 3;
			public int current;
		}

		private static DialogueManager instance = null;
		public static DialogueManager Instance 
		{ 
			get 
			{
				if(instance == null)
				{
					GameObject go = new GameObject();
					instance = go.AddComponent<DialogueManager>();
					go.name = "DialogueManager";
				}

				return instance; 
			} 
		}

		void Awake()
		{
			if(instance == null)
			{
				instance = this;
			}
		}

		void Start()
		{
			willStopTimeOnShow = GameManager.Instance.settings.willStopTimeOnDialogueShow;
			Hide();
		}

		void Update()
		{
			if(text.gameObject.activeSelf)
			{
				if((GameManager.Instance.input.isJumpButtonDownThisFrame || Input.GetMouseButtonDown(0)))
				{
					if(isCloseEnabled)
					{
						if(pageInfo.current < pageInfo.total - 1)
						{
							StopCoroutine("ShowCoroutine");
							AdvancePage();
						}
						else
						{
							Hide();
						}
					}
					else
					{
						willSkipToEnd = true;
					}
				}
			}
		}

		public void ShowRawText(string _text)
		{
			List<DialogueManager.Page> pages = new List<DialogueManager.Page>();
			DialogueManager.Page page = new DialogueManager.Page();
			page.text = _text;
			pages.Add(page);

			Show(pages);
		}

		public void Show(List<Page> _pages)
		{
			if(_pages == null || _pages.Count < 1)
			{
				return;
			}

			#if UNITY_ANDROID || UNITY_IPHONE
			RexTouchInput rexTouchInput = GameManager.Instance.player.GetComponent<RexTouchInput>();
			if(rexTouchInput != null)
			{
				rexTouchInput.ToggleTouchInterface(false);
			}
			#endif

			pages = _pages;
			pageInfo.current = 0;
			pageInfo.total = pages.Count;

			isDialogueActive = true;

			if(willStopTimeOnShow)
			{
				foreach(RexActor actor in GameObject.FindObjectsOfType<RexActor>())
				{
					actor.StopTime();
				}

			}

			DisplayPage(pageInfo.current);
		}

		protected void DisplayPage(int _page)
		{
			if(audioSource && showSound)
			{
				audioSource.PlayOneShot(showSound);
			}

			willSkipToEnd = false;
			text.text = pages[pageInfo.current].text;
			text.gameObject.SetActive(true);
			GameManager.Instance.player.RemoveControl();
			isCloseEnabled = false;
			advanceIcon.SetActive(false);
			StartCoroutine("ShowCoroutine");
		}

		protected void AdvancePage()
		{
			pageInfo.current ++;
			DisplayPage(pageInfo.current);
		}

		public void Hide()
		{
			#if UNITY_ANDROID || UNITY_IPHONE
			RexTouchInput rexTouchInput = GameManager.Instance.player.GetComponent<RexTouchInput>();
			if(rexTouchInput != null)
			{
				rexTouchInput.ToggleTouchInterface(true);
			}
			#endif

			if(willStopTimeOnShow)
			{
				foreach(RexActor actor in GameObject.FindObjectsOfType<RexActor>())
				{
					if(actor.timeStop.isTimeStopped)
					{
						actor.StartTime();
					}
				}
			}

			if(audioSource && hideSound && text.gameObject.activeSelf)
			{
				audioSource.PlayOneShot(hideSound);
			}

			text.text = "";
			text.gameObject.SetActive(false);
			StartCoroutine("HideCoroutine");
		}

		public bool IsDialogueActive()
		{
			return isDialogueActive;
		}

		protected IEnumerator ShowCoroutine()
		{
			wordsInText = text.text.Split(' ');
			text.text = "";
			currentlyDisplayedText = "";

			int currentWord = 0;
			int currentLetter = 0;
			while(currentWord < wordsInText.Length)
			{
				//bool didInsertLinebreak = false;
				string textIncludingNextWord = (currentWord <= wordsInText.Length - 1) ? text.text + wordsInText[currentWord + 0] : text.text;
				float textWidth = GetLineWidth(text, textIncludingNextWord);

				if(textWidth >= dialogueSettings.textFieldWidth)
				{
					currentlyDisplayedText += "\n";
				}

				currentLetter = 0;
				char[] letters = wordsInText[currentWord].ToCharArray();
				while(currentLetter < letters.Length)
				{
					text.text = currentlyDisplayedText + letters[currentLetter];

					if(!willSkipToEnd)
					{
						yield return new WaitForSeconds(dialogueSettings.delayBetweenLetters);
					}

					currentlyDisplayedText += letters[currentLetter];
					currentLetter ++;
				}

				currentlyDisplayedText += ' ';

				currentWord ++;
			}

			yield return new WaitForSeconds(0.5f);

			SkipTextAhead();
		}

		protected void SkipTextAhead()
		{
			advanceIcon.SetActive(true);
			isCloseEnabled = true;
			StopCoroutine("ShowCoroutine");
		}

		protected IEnumerator HideCoroutine()
		{
			yield return new WaitForSeconds(0.1f);

			isDialogueActive = false;
			GameManager.Instance.player.RegainControl();
			isCloseEnabled = true;

			if(onDialogueComplete != null)
			{
				onDialogueComplete.Invoke();
			}
		}

		public float GetLineWidth(TextMesh mesh, string text)
		{
			float width = 0;
			string[] lines = text.Split('\n');
			foreach(char symbol in lines[lines.Length - 1])
			{
				CharacterInfo info;
				if(mesh.font.GetCharacterInfo(symbol, out info, mesh.fontSize, mesh.fontStyle))
				{
					width += info.advance;
				}
			}

			return width * mesh.characterSize * 0.1f;
		}
	}
}
