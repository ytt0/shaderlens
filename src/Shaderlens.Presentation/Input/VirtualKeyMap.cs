namespace Shaderlens.Presentation.Input
{
    public static class VirtualKeyMap
    {
        private static readonly Lazy<Dictionary<Key, uint>> Map = new Lazy<Dictionary<Key, uint>>(CreateKeyMap);

        private static Dictionary<Key, uint> CreateKeyMap()
        {
            // https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes

            var map = new Dictionary<Key, uint>
            {
                 { Key.Back, 0x08 }, // BACKSPACE key
                 { Key.Tab, 0x09 }, // TAB key
                 { Key.Clear, 0x0C }, // CLEAR key
                 { Key.Return, 0x0D }, // ENTER key
                 { Key.Pause, 0x13 }, // PAUSE key
                 { Key.Capital, 0x14 }, // CAPS LOCK key
                 { Key.KanaMode, 0x15 }, // IME Kana mode
                 //{ Key.HangulMode, 0x15 }, // IME Hangul mode
                 { Key.JunjaMode, 0x17 }, // IME Junja mode
                 { Key.FinalMode, 0x18 }, // IME final mode
                 { Key.HanjaMode, 0x19 }, // IME Hanja mode
                 //{ Key.KanjiMode, 0x19 }, // IME Kanji mode
                 { Key.Escape, 0x1B }, // ESC key
                 { Key.ImeConvert, 0x1C }, // IME convert
                 { Key.ImeNonConvert, 0x1D }, // IME nonconvert
                 { Key.ImeAccept, 0x1E }, // IME accept
                 { Key.ImeModeChange, 0x1F }, // IME mode change request
                 { Key.Space, 0x20 }, // SPACEBAR
                 { Key.Prior, 0x21 }, // PAGE UP key
                 { Key.Next, 0x22 }, // PAGE DOWN key
                 { Key.End, 0x23 }, // END key
                 { Key.Home, 0x24 }, // HOME key
                 { Key.Left, 0x25 }, // LEFT ARROW key
                 { Key.Up, 0x26 }, // UP ARROW key
                 { Key.Right, 0x27 }, // RIGHT ARROW key
                 { Key.Down, 0x28 }, // DOWN ARROW key
                 { Key.Select, 0x29 }, // SELECT key
                 { Key.Print, 0x2A }, // PRINT key
                 { Key.Execute, 0x2B }, // EXECUTE key
                 { Key.Snapshot, 0x2C }, // PRINT SCREEN key
                 { Key.Insert, 0x2D }, // INS key
                 { Key.Delete, 0x2E }, // DEL key
                 { Key.Help, 0x2F }, // HELP key
                 { Key.D0, 0x30 }, // 0 key
                 { Key.D1, 0x31 }, // 1 key
                 { Key.D2, 0x32 }, // 2 key
                 { Key.D3, 0x33 }, // 3 key
                 { Key.D4, 0x34 }, // 4 key
                 { Key.D5, 0x35 }, // 5 key
                 { Key.D6, 0x36 }, // 6 key
                 { Key.D7, 0x37 }, // 7 key
                 { Key.D8, 0x38 }, // 8 key
                 { Key.D9, 0x39 }, // 9 key
                 { Key.A, 0x41 }, // A key
                 { Key.B, 0x42 }, // B key
                 { Key.C, 0x43 }, // C key
                 { Key.D, 0x44 }, // D key
                 { Key.E, 0x45 }, // E key
                 { Key.F, 0x46 }, // F key
                 { Key.G, 0x47 }, // G key
                 { Key.H, 0x48 }, // H key
                 { Key.I, 0x49 }, // I key
                 { Key.J, 0x4A }, // J key
                 { Key.K, 0x4B }, // K key
                 { Key.L, 0x4C }, // L key
                 { Key.M, 0x4D }, // M key
                 { Key.N, 0x4E }, // N key
                 { Key.O, 0x4F }, // O key
                 { Key.P, 0x50 }, // P key
                 { Key.Q, 0x51 }, // Q key
                 { Key.R, 0x52 }, // R key
                 { Key.S, 0x53 }, // S key
                 { Key.T, 0x54 }, // T key
                 { Key.U, 0x55 }, // U key
                 { Key.V, 0x56 }, // V key
                 { Key.W, 0x57 }, // W key
                 { Key.X, 0x58 }, // X key
                 { Key.Y, 0x59 }, // Y key
                 { Key.Z, 0x5A }, // Z key
                 { Key.LWin, 0x5B }, // Left Windows key
                 { Key.RWin, 0x5C }, // Right Windows key
                 { Key.Apps, 0x5D }, // Applications key
                 { Key.Sleep, 0x5F }, // Computer Sleep key
                 { Key.NumPad0, 0x60 }, // Numeric keypad 0 key
                 { Key.NumPad1, 0x61 }, // Numeric keypad 1 key
                 { Key.NumPad2, 0x62 }, // Numeric keypad 2 key
                 { Key.NumPad3, 0x63 }, // Numeric keypad 3 key
                 { Key.NumPad4, 0x64 }, // Numeric keypad 4 key
                 { Key.NumPad5, 0x65 }, // Numeric keypad 5 key
                 { Key.NumPad6, 0x66 }, // Numeric keypad 6 key
                 { Key.NumPad7, 0x67 }, // Numeric keypad 7 key
                 { Key.NumPad8, 0x68 }, // Numeric keypad 8 key
                 { Key.NumPad9, 0x69 }, // Numeric keypad 9 key
                 { Key.Multiply, 0x6A }, // Multiply key
                 { Key.Add, 0x6B }, // Add key
                 { Key.Separator, 0x6C }, // Separator key
                 { Key.Subtract, 0x6D }, // Subtract key
                 { Key.Decimal, 0x6E }, // Decimal key
                 { Key.Divide, 0x6F }, // Divide key
                 { Key.F1, 0x70 }, // F1 key
                 { Key.F2, 0x71 }, // F2 key
                 { Key.F3, 0x72 }, // F3 key
                 { Key.F4, 0x73 }, // F4 key
                 { Key.F5, 0x74 }, // F5 key
                 { Key.F6, 0x75 }, // F6 key
                 { Key.F7, 0x76 }, // F7 key
                 { Key.F8, 0x77 }, // F8 key
                 { Key.F9, 0x78 }, // F9 key
                 { Key.F10, 0x79 }, // F10 key
                 { Key.F11, 0x7A }, // F11 key
                 { Key.F12, 0x7B }, // F12 key
                 { Key.F13, 0x7C }, // F13 key
                 { Key.F14, 0x7D }, // F14 key
                 { Key.F15, 0x7E }, // F15 key
                 { Key.F16, 0x7F }, // F16 key
                 { Key.F17, 0x80 }, // F17 key
                 { Key.F18, 0x81 }, // F18 key
                 { Key.F19, 0x82 }, // F19 key
                 { Key.F20, 0x83 }, // F20 key
                 { Key.F21, 0x84 }, // F21 key
                 { Key.F22, 0x85 }, // F22 key
                 { Key.F23, 0x86 }, // F23 key
                 { Key.F24, 0x87 }, // F24 key
                 { Key.NumLock, 0x90 }, // NUM LOCK key
                 { Key.Scroll, 0x91 }, // SCROLL LOCK key
                 { Key.LeftShift, 0xA0 }, // Left SHIFT key
                 { Key.RightShift, 0xA1 }, // Right SHIFT key
                 { Key.LeftCtrl, 0xA2 }, // Left CONTROL key
                 { Key.RightCtrl, 0xA3 }, // Right CONTROL key
                 { Key.LeftAlt, 0xA4 }, // Left ALT key
                 { Key.RightAlt, 0xA5 }, // Right ALT key
                 { Key.BrowserBack, 0xA6 }, // Browser Back key
                 { Key.BrowserForward, 0xA7 }, // Browser Forward key
                 { Key.BrowserRefresh, 0xA8 }, // Browser Refresh key
                 { Key.BrowserStop, 0xA9 }, // Browser Stop key
                 { Key.BrowserSearch, 0xAA }, // Browser Search key
                 { Key.BrowserFavorites, 0xAB }, // Browser Favorites key
                 { Key.BrowserHome, 0xAC }, // Browser Start and Home key
                 { Key.VolumeMute, 0xAD }, // Volume Mute key
                 { Key.VolumeDown, 0xAE }, // Volume Down key
                 { Key.VolumeUp, 0xAF }, // Volume Up key
                 { Key.MediaNextTrack, 0xB0 }, // Next Track key
                 { Key.MediaPreviousTrack, 0xB1 }, // Previous Track key
                 { Key.MediaStop, 0xB2 }, // Stop Media key
                 { Key.MediaPlayPause, 0xB3 }, // Play/Pause Media key
                 { Key.LaunchMail, 0xB4 }, // Start Mail key
                 { Key.SelectMedia, 0xB5 }, // Select Media key
                 { Key.LaunchApplication1, 0xB6 }, // Start Application 1 key
                 { Key.LaunchApplication2, 0xB7 }, // Start Application 2 key
                 { Key.Oem1, 0xBA }, // Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ;: key
                 { Key.OemPlus, 0xBB }, // For any country/region, the + key
                 { Key.OemComma, 0xBC }, // For any country/region, the , key
                 { Key.OemMinus, 0xBD }, // For any country/region, the - key
                 { Key.OemPeriod, 0xBE }, // For any country/region, the . key
                 { Key.Oem2, 0xBF }, // Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the /? key
                 { Key.Oem3, 0xC0 }, // Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the `~ key
                 { Key.Oem4, 0xDB }, // Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the [{ key
                 { Key.Oem5, 0xDC }, // Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the \\| key
                 { Key.Oem6, 0xDD }, // Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the ]} key
                 { Key.Oem7, 0xDE }, // Used for miscellaneous characters; it can vary by keyboard. For the US standard keyboard, the '" key
                 { Key.Oem8, 0xDF }, // Used for miscellaneous characters; it can vary by keyboard.
                 { Key.Oem102, 0xE2 }, // The <> keys on the US standard keyboard, or the \\| key on the non-US 102-key keyboard
                 { Key.ImeProcessed, 0xE5 }, // IME PROCESS key
                 { Key.Attn, 0xF6 }, // Attn key
                 { Key.CrSel, 0xF7 }, // CrSel key
                 { Key.ExSel, 0xF8 }, // ExSel key
                 { Key.EraseEof, 0xF9 }, // Erase EOF key
                 { Key.Play, 0xFA }, // Play key
                 { Key.Zoom, 0xFB }, // Zoom key
                 { Key.Pa1, 0xFD }, // PA1 key
                 { Key.OemClear, 0xFE }, // Clear key
            };

            return map;
        }

        public static bool TryGetIndex(Key key, out uint index)
        {
            return Map.Value.TryGetValue(key, out index);
        }
    }
}
