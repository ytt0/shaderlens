namespace Shaderlens.Presentation.Input
{
    public interface IInputValueSerializer
    {
        string Serialize(Key key);
        string Serialize(ModifierKey modifierKey);
        string Serialize(MouseButton button);
        string Serialize(MouseScroll direction);
        bool TryDeserialize(string value, out Key key);
        bool TryDeserialize(string value, out ModifierKey modifierKey);
        bool TryDeserialize(string value, out MouseButton mouseButton);
        bool TryDeserialize(string value, out MouseScroll direction);
        string GetValuesDescription();
    }

    public class InputValueSerializer : IInputValueSerializer
    {
        public string Serialize(Key key)
        {
            return KeyMapping.TryGetValue(key, out var value) ? value : key.ToString();
        }

        public string Serialize(ModifierKey modifierKey)
        {
            return modifierKey.ToString();
        }

        public string Serialize(MouseButton button)
        {
            return MouseMapping[button];
        }

        public string Serialize(MouseScroll direction)
        {
            return direction.ToString();
        }

        public bool TryDeserialize(string value, out Key key)
        {
            return KeyReverseMapping.TryGetValue(value, out key) || Enum.TryParse(value, out key);
        }

        public bool TryDeserialize(string value, out ModifierKey modifierKey)
        {
            return Enum.TryParse(value, out modifierKey);
        }

        public bool TryDeserialize(string value, out MouseButton mouseButton)
        {
            return MouseReverseMapping.TryGetValue(value, out mouseButton);
        }

        public bool TryDeserialize(string value, out MouseScroll direction)
        {
            return Enum.TryParse(value, out direction);
        }

        public string GetValuesDescription()
        {
            var stringBuilder = new StringBuilder();
            var includedKeys = KeyMapping.Where(pair => !(pair.Key >= Key.D0 && pair.Key <= Key.Z || pair.Key >= Key.F1 && pair.Key <= Key.F12 || pair.Key >= Key.NumPad0 && pair.Key <= Key.NumPad9)).Select(pair => pair.Value).ToArray();
            const int includedKeysSplit = 18;

            stringBuilder.AppendLine($"// Keys: A..Z 0..9 Num0..Num9 F1..F12 {String.Join(' ', Enum.GetValues<ModifierKey>().Select(Serialize))} {String.Join(' ', includedKeys.Take(includedKeysSplit))}");
            stringBuilder.AppendLine($"//       {String.Join(' ', includedKeys.Skip(includedKeysSplit))}");
            stringBuilder.AppendLine($"// Mouse: {String.Join(' ', MouseMapping.Values.Concat(Enum.GetValues<MouseScroll>().Select(Serialize)))}");
            return stringBuilder.ToString();
        }

        private static readonly IReadOnlyDictionary<MouseButton, string> MouseMapping = new Dictionary<MouseButton, string>
        {
            { MouseButton.Left, "MouseLeft" },
            { MouseButton.Middle, "MouseMiddle" },
            { MouseButton.Right, "MouseRight" },
            { MouseButton.XButton1, "MouseX1" },
            { MouseButton.XButton2, "MouseX2" },
        };

        private static readonly Dictionary<string, MouseButton> MouseReverseMapping = MouseMapping.ToDictionary(pair => pair.Value, pair => pair.Key);

        private static readonly Dictionary<Key, string> KeyMapping = new Dictionary<Key, string>
        {
            { Key.Return, "Enter" },
            { Key.Space, "Space" },
            { Key.Escape, "Esc" },
            { Key.Tab, "Tab" },
            { Key.A, "A" },
            { Key.B, "B" },
            { Key.C, "C" },
            { Key.D, "D" },
            { Key.E, "E" },
            { Key.F, "F" },
            { Key.G, "G" },
            { Key.H, "H" },
            { Key.I, "I" },
            { Key.J, "J" },
            { Key.K, "K" },
            { Key.L, "L" },
            { Key.M, "M" },
            { Key.N, "N" },
            { Key.O, "O" },
            { Key.P, "P" },
            { Key.Q, "Q" },
            { Key.R, "R" },
            { Key.S, "S" },
            { Key.T, "T" },
            { Key.U, "U" },
            { Key.V, "V" },
            { Key.W, "W" },
            { Key.X, "X" },
            { Key.Y, "Y" },
            { Key.Z, "Z" },
            { Key.OemPlus, "+" },
            { Key.OemMinus, "-" },
            { Key.OemComma, "<" },
            { Key.OemPeriod, ">" },
            { Key.OemOpenBrackets, "[" },
            { Key.OemCloseBrackets, "]" },
            { Key.OemQuestion, "/" },
            { Key.OemPipe, "\\" },
            { Key.OemSemicolon, ";" },
            { Key.OemQuotes, "'" },
            { Key.OemTilde, "~" },
            { Key.D0, "0" },
            { Key.D1, "1" },
            { Key.D2, "2" },
            { Key.D3, "3" },
            { Key.D4, "4" },
            { Key.D5, "5" },
            { Key.D6, "6" },
            { Key.D7, "7" },
            { Key.D8, "8" },
            { Key.D9, "9" },
            { Key.Back, "Backspace" },
            { Key.Insert, "Insert" },
            { Key.Delete, "Delete" },
            { Key.Home, "Home" },
            { Key.End, "End" },
            { Key.PageUp, "PageUp" },
            { Key.PageDown, "PageDown" },
            { Key.Left, "Left" },
            { Key.Right, "Right" },
            { Key.Up, "Up" },
            { Key.Down, "Down" },
            { Key.NumPad0, "Num0" },
            { Key.NumPad1, "Num1" },
            { Key.NumPad2, "Num2" },
            { Key.NumPad3, "Num3" },
            { Key.NumPad4, "Num4" },
            { Key.NumPad5, "Num5" },
            { Key.NumPad6, "Num6" },
            { Key.NumPad7, "Num7" },
            { Key.NumPad8, "Num8" },
            { Key.NumPad9, "Num9" },
            { Key.Add, "Num+" },
            { Key.Subtract, "Num-" },
            { Key.Multiply, "Num*" },
            { Key.Divide, "Num/" },
            { Key.Decimal, "Num." },
            { Key.F1, "F1" },
            { Key.F2, "F2" },
            { Key.F3, "F3" },
            { Key.F4, "F4" },
            { Key.F5, "F5" },
            { Key.F6, "F6" },
            { Key.F7, "F7" },
            { Key.F8, "F8" },
            { Key.F9, "F9" },
            { Key.F10, "F10" },
            { Key.F11, "F11" },
            { Key.F12, "F12" },
            { Key.LeftAlt, "LAlt" },
            { Key.RightAlt, "RAlt" },
            { Key.LeftCtrl, "LCtrl" },
            { Key.RightCtrl, "RCtrl" },
            { Key.LeftShift, "LShift" },
            { Key.RightShift, "RShift" },
            { Key.Apps, "Menu" },
            { Key.Pause, "Pause" },
        };

        private static readonly Dictionary<string, Key> KeyReverseMapping = KeyMapping.ToDictionary(pair => pair.Value, pair => pair.Key);
    }
}
