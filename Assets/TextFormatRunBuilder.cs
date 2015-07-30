using System.Text;

public class TextFormatRunBuilder {
	string lastColor;
	bool needsClose;
	readonly StringBuilder builder = new StringBuilder();

	public void AddChar(char ch, string color) {
		if (color != lastColor) {
			if (lastColor != null) builder.Append("</color>");
			builder.Append("<color=" + color + ">");
			lastColor = color;
		}

		builder.Append(ch);
		needsClose = true;
	}

	public void AddString(string s) {
		builder.Append(s);
	}

	public void Clear() {
		lastColor = null;
		builder.Length = 0;
	}

	public override string ToString() {
		if (needsClose) {
			needsClose = false;
			builder.Append("</color>");
		}

		return builder.ToString();
	}
}

