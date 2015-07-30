using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Text;

public class RevealText : MonoBehaviour {
	Text text;

	[Range(0, 1.0f)] public float waitBetweenChars = 0.1f;
	[Range(0, 1.0f)] public float waitBetweenCharsHigh = 0.15f;

	public float fadeWidthChars = 6.0f;

	public float sentenceDelayLow = 0.15f;
	public float sentenceDelayHigh = 0.20f;

	public AudioSource typingSound;

	public float cursorBlinkTime = 0.5f;
	public string cursor = "|";
	public bool cursorEnabled = true;

	public string pauseChars = ".,;";

	public bool revealOnEnable;

	public bool cursorVisible { get { return cursorEnabled && (Time.time % cursorBlinkTime < cursorBlinkTime / 2.0); } }

	public string WithCursor(string s) { return cursorVisible ? (s + cursor) : s; }

	public void OnEnable() {
		text = GetComponent<Text>();
		if (text == null) {
			Debug.LogError ("NO TEXT " + this);
		} else if (revealOnEnable) {
			Reveal (text.text);
		}
	}

	static bool IsControlChar(char[] chars, char controlChar, int index) {
		return chars[index] == '^' &&
			(index + 1 < chars.Length) &&
			chars[index + 1] == controlChar;
	}

	// TODO: this belongs elsewhere. ConversationNode?
	public static string ParseControlChars(string text) {
		var messageChars = text.ToCharArray();
		var chars = new List<char>();

		for (int i = 0; i < text.Length; ++i) {
			if (IsControlChar(messageChars, 'H', i)) {
				if (chars.Count > 0)
					chars.RemoveAt(chars.Count - 1);
				i++;
			} else if (IsControlChar(messageChars, 'p', i) ||
					   IsControlChar(messageChars, 'P', i)) {
				i++;
			} else {
				chars.Add(messageChars[i]);
			}
		}
		
		return new string(chars.ToArray());
	}

	public void Reveal2(string originalText, Action onComplete = null) {
		StartCoroutine(DoRevealCoro2(originalText, "", onComplete));
	}

	public static string AddStringDelays(string s) {
		s = Regex.Replace(s, @"\.\.\.", "$0~~~~~~~~~~~~~~~");
		s = Regex.Replace(s, @"((?:\.|\?)\]?)([^.])?", "$1~~~~~~~~~~~$2");
		s = Regex.Replace(s, @"(?:\?|\!)[^\]]", "?~~~~~~~~~~~~~~~");
		s = s
			.Replace(",", ",~~~~~~~~~~~~~")
			.Replace("^P", "~~~~~~~~~~")
			.Replace("^p", "~~~~~")
		;
		return s;
	}

	public AnimationCurve animCurve;
	float timePerCharacter = 0.025f;
	char[] originalChars;
	TextFormatRunBuilder runBuilder = new TextFormatRunBuilder();

	public IEnumerator DoRevealCoro2(string originalText, string prefix="", Action onComplete = null) {
		if (prefix.Length > 0) {
			if (!originalText.StartsWith(prefix))
				Debug.LogError("does not start with prefix");
			originalText = originalText.Substring(prefix.Length);
		}

		// tildes become virtual delays
		originalChars = AddStringDelays(originalText).ToCharArray();

		float startTime = Time.time;
		float totalTime = timePerCharacter * (float)originalChars.Length;
		float fadeWidth = fadeWidthChars / (float)originalChars.Length;
		float finished;

		string color = text.color.HexString();

		do {
			var now = Time.time;
			finished = Mathf.Clamp01((now - startTime) / totalTime);

			var fadeStart = finished;

			runBuilder.Clear();
			runBuilder.AddString(prefix);
			var charsLength = originalChars.Length;
			for (int i = 0; i < charsLength; ++i) {
				if (originalChars[i] == '~') continue; // TODO: use a more obscure unicode character, so tilde is free
				float charPercent = (float)i / (float)charsLength;
				var val = Mathf.Clamp01((charPercent - fadeStart) / fadeWidth);
				float alpha = 1.0f - Mathf.Clamp01(animCurve != null ? animCurve.Evaluate(val) : val);
				var alphaHex = ((int)(alpha * 255.0f)).ToString("X2"); // TODO @GC
				runBuilder.AddChar(originalChars[i], color + alphaHex);
			}

			text.text = runBuilder.ToString();
			yield return null;
		} while (finished < 1);

		if (onComplete != null)
			onComplete();
	}

	public void Reveal(string originalText) {
		StartCoroutine(DoRevealCoro(originalText));
	}

	IEnumerator WaitForSecondsWithCursor(float secs) {
		var originalText = text.text;
		var targetTime = Time.time + secs;
		while (Time.time < targetTime) {
			text.text = WithCursor(originalText);
			yield return null;
		}
		text.text = originalText;
	}

	IEnumerator DoRevealCoro(string originalText, Action onComplete = null) {
		char[] chars = originalText.ToCharArray();
		var parts = new List<string>();

		for (int revealIdx = 0; revealIdx < originalText.Length; ++revealIdx) {
			var delay = UnityEngine.Random.Range(waitBetweenChars, waitBetweenCharsHigh);
			yield return StartCoroutine(WaitForSecondsWithCursor(delay));

			char lastChar = ' ';

			if (IsControlChar(chars, 'H', revealIdx)) { // backspace
				revealIdx += 1;
				parts.RemoveAt(parts.Count - 1);
			} else if (IsControlChar(chars, 'P', revealIdx)) { // pause
				revealIdx += 1;
				yield return StartCoroutine(WaitForSecondsWithCursor(1.25f));
			} else if (IsControlChar(chars, 'p', revealIdx)) { // small pause
				revealIdx += 1;
				yield return StartCoroutine(WaitForSecondsWithCursor(0.5f));
			} else {
				lastChar = chars[revealIdx];
				if (typingSound != null)
					typingSound.Play();
				parts.Add(lastChar.ToString());
			}

			text.text = string.Join("", parts.ToArray());

			foreach (char ch in pauseChars) {
				if (lastChar == ch) {
					yield return StartCoroutine(WaitForSecondsWithCursor(UnityEngine.Random.Range(sentenceDelayLow, sentenceDelayHigh)));
					break;
				}
			}
		}

		var parsed = ParseControlChars(originalText);
		if (text.text != parsed) {
			Debug.Log("ParseControlChars did not achieve the same result as the iterative coroutine:");
			Debug.Log("  text:   '" + text.text + "'");
			Debug.Log("  parsed: '" + parsed + "'");
		}

		text.text = string.Join("", parts.ToArray());

		if (onComplete != null)
			onComplete();
	}

	/*
	List<UICharInfo> characters;

	public bool PositionOfText(string s, out Vector2 position) {
		position = Vector2.zero;

		int index = text.text.IndexOf(s, StringComparison.Ordinal);
		if (index == -1) return false;

		// HACK: private API
		var textGenerator = text.GetFieldValue("m_TextCache") as TextGenerator;
		if (textGenerator == null) return false;

		if (characters == null) characters = new List<UICharInfo>();
		textGenerator.GetCharacters(characters);

		Debug.Log(textGenerator.rectExtents);

		position = characters[index].cursorPos;

		return true;
	}
	*/

}
