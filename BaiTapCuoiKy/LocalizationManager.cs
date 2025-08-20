using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace BaiTapCuoiKy
{
    public enum AppLanguage
    {
        Vietnamese,
        English
    }

    public static class LocalizationManager
    {
        private static AppLanguage _language = AppLanguage.Vietnamese;
        public static AppLanguage Language => _language;

        public static event Action LanguageChanged;

        private static readonly Dictionary<string, (string vi, string en)> _texts = new Dictionary<string, (string vi, string en)>
        {
            ["APP_TITLE"] = ("DrawMaster - Chào m?ng", "DrawMaster - Welcome"),
            ["APP_TITLE_LOGIN"] = ("DrawMaster - ??ng nh?p", "DrawMaster - Login"),

            // Welcome/Main
            ["WELCOME_TITLE"] = ("?? DRAWMASTER PREMIUM ??", "?? DRAWMASTER PREMIUM ??"),
            ["WELCOME_SUBTITLE"] = ("?? Tr?i nghi?m V? và ?oán nhi?u ng??i ch?i ??", "?? Multiplayer Drawing & Guessing Experience ??"),
            ["WELCOME_HELLO"] = ("?? Chào m?ng, {0}! ??", "?? Welcome, {0}! ??"),
            ["WELCOME_OPTIONS"] = ("?? Ch?n m?t tùy ch?n ?? b?t ??u cu?c phiêu l?u:", "?? Choose an option to start the adventure:"),
            ["LANGUAGE"] = ("Ngôn ng?:", "Language:"),

            // Buttons/common
            ["BTN_CREATE_ROOM"] = ("?? T?O PHÒNG M?I", "?? CREATE ROOM"),
            ["BTN_JOIN"] = ("?? THAM GIA", "?? JOIN"),
            ["BTN_LOGOUT"] = ("?? ??NG XU?T", "?? LOG OUT"),
            ["BTN_LEAVE_LOBBY"] = ("?? R?i Lobby", "?? Leave Lobby"),
            ["BTN_INVITE"] = ("?? M?i b?n bè", "?? Invite friends"),
            ["BTN_START_GAME"] = ("?? B?T ??U GAME", "?? START GAME"),

            // Lobby
            ["LBL_JOIN_IP"] = ("Nh?p IP phòng:", "Enter room IP:"),
            ["STATS_BRIEF"] = ("?? Tr?n ?ã ch?i: 0  |  ? ?i?m cao nh?t: 0  |  ?? Th?ng: 0", "?? Games: 0  |  ? Best: 0  |  ?? Wins: 0"),
            ["LOBBY_TITLE"] = ("LOBBY - PHÒNG: {0}", "LOBBY - ROOM: {0}"),
            ["LOBBY_PLAYERS"] = ("?? Ng??i ch?i trong phòng", "?? Players in room"),
            ["LOBBY_CHAT"] = ("?? Chat s?nh ch?", "?? Lobby chat"),
            ["LOBBY_WAITING"] = ("?ang ch? ng??i ch?i...", "Waiting for players..."),
            ["LOBBY_READY"] = ("? ?? ng??i ch?i - Có th? b?t ??u game!", "? Enough players - Ready to start!"),
            ["LOBBY_NEED_MORE"] = ("? C?n thêm ng??i ch?i ?? b?t ??u...", "? Need more players to start..."),
            ["INVITE_MESSAGE"] = ("?? M?i b?n bè tham gia game!\n\n?? ??a ch? k?t n?i: {0}\n?? Copy ??a ch? này và g?i cho b?n bè ?? h? có th? tham gia!", "?? Invite your friends!\n\n?? Connection address: {0}\n?? Copy this address and share to let them join!"),
            ["COPIED_CLIPBOARD"] = ("?? ??a ch? k?t n?i ?ã ???c copy vào clipboard!", "?? Connection address copied to clipboard!"),
            ["CONFIRM_LEAVE_LOBBY_TITLE"] = ("Xác nh?n r?i lobby", "Confirm leave lobby"),
            ["CONFIRM_LEAVE_LOBBY_MSG"] = ("B?n có ch?c ch?n mu?n r?i kh?i lobby không?", "Are you sure you want to leave the lobby?"),
            ["NOT_ENOUGH_PLAYERS"] = ("C?n ít nh?t 2 ng??i ch?i ?? b?t ??u game!", "At least 2 players are required to start!"),
            ["CONFIRM_START_TITLE"] = ("Xác nh?n b?t ??u game", "Confirm start"),
            ["CONFIRM_START_MSG"] = ("?? B?t ??u game v?i {0} ng??i ch?i?\n\n?? Sau khi b?t ??u, không th? thêm ng??i ch?i m?i!", "?? Start game with {0} players?\n\n?? After starting, no new players can join!"),

            // Game chat/UI
            ["CHAT_WELCOME_ROOM"] = ("Chào m?ng ??n phòng {0}!", "Welcome to room {0}!"),
            ["CHAT_PRESS_START"] = ("Nh?n 'B?t ??u' ?? vào l??t v? và ch?i!", "Press 'Start' to begin drawing and playing!"),
            ["TIME_UP"] = ("? H?t gi?!", "? Time's up!"),
            ["ANOTHER_PLAYER_DRAWING"] = ("M?t ng??i ch?i khác ?ang v?...", "Another player is drawing..."),
            ["GAME_OVER"] = ("?? Trò ch?i k?t thúc! C?m ?n ?ã ch?i!", "?? Game over! Thanks for playing!"),

            // Login form
            ["LOGIN_BRAND_TITLE"] = ("DrawMaster", "DrawMaster"),
            ["LOGIN_BRAND_SUBTITLE"] = ("Trò ch?i V? nhi?u ng??i ch?i", "Multiplayer Drawing Game"),
            ["LOGIN_FEATURES"] = ("?? V? và ?oán cùng b?n bè\n??? Công c? v? chuyên nghi?p\n?? Chat th?i gian th?c\n?? H? th?ng ?i?m s?\n?? H? tr? ?a ngôn ng?", "?? Draw & guess with friends\n??? Pro drawing tools\n?? Real-time chat\n?? Scoring system\n?? Multi-language support"),
            ["LOGIN_TITLE"] = ("??NG NH?P", "LOGIN"),
            ["REGISTER_TITLE"] = ("??NG KÝ", "REGISTER"),
            ["USERNAME_OR_EMAIL"] = ("Tên ??ng nh?p ho?c Email:", "Username or Email:"),
            ["USERNAME_ONLY"] = ("Tên ??ng nh?p:", "Username:"),
            ["PASSWORD"] = ("M?t kh?u:", "Password:"),
            ["CONFIRM_PASSWORD"] = ("Xác nh?n m?t kh?u:", "Confirm password:"),
            ["EMAIL"] = ("Email:", "Email:"),
            ["BTN_LOGIN"] = ("??NG NH?P", "LOGIN"),
            ["BTN_REGISTER"] = ("??NG KÝ", "REGISTER"),
            ["OR_LOGIN_WITH"] = ("HO?C ??NG NH?P B?NG", "OR SIGN IN WITH"),
            ["TOGGLE_TO_REGISTER"] = ("Ch?a có tài kho?n? ??ng ký ngay", "Don't have an account? Register now"),
            ["TOGGLE_TO_LOGIN"] = ("?ã có tài kho?n? ??ng nh?p", "Already have an account? Login"),
            ["FILL_ALL_FIELDS"] = ("Vui lòng nh?p ??y ?? thông tin!", "Please fill in all fields!"),
            ["LOGIN_SUCCESS"] = ("??ng nh?p thành công! Chào m?ng {0}!", "Login successful! Welcome {0}!"),
            ["LOGIN_FAILED"] = ("Tên ??ng nh?p ho?c m?t kh?u không ?úng!", "Invalid username or password!"),
            ["PASSWORD_MISMATCH"] = ("M?t kh?u xác nh?n không kh?p!", "Password confirmation does not match!"),
            ["INVALID_EMAIL"] = ("Email không h?p l?!", "Invalid email!"),
            ["USERNAME_EXISTS"] = ("Tên ??ng nh?p ?ã t?n t?i!", "Username already exists!"),
            ["EMAIL_IN_USE"] = ("Email ?ã ???c s? d?ng!", "Email is already in use!"),
            ["REGISTER_SUCCESS"] = ("??ng ký thành công! B?n có th? ??ng nh?p ngay bây gi?.", "Registration successful! You can login now."),
            ["SOCIAL_LOGIN_SUCCESS"] = ("??ng nh?p {0} thành công!", "{0} login successful!"),
            ["ERROR"] = ("L?i", "Error"),
            ["ERROR_LOAD_ACCOUNTS"] = ("L?i khi t?i d? li?u tài kho?n: {0}", "Error loading accounts: {0}"),
            ["ERROR_SAVE_ACCOUNTS"] = ("L?i khi l?u d? li?u tài kho?n: {0}", "Error saving accounts: {0}"),
        };

        public static string Tr(string key)
        {
            if (_texts.TryGetValue(key, out var tuple))
            {
                return _language == AppLanguage.English ? tuple.en : tuple.vi;
            }
            return key;
        }

        public static string TrFormat(string key, params object[] args)
        {
            return string.Format(Tr(key), args);
        }

        public static void SetLanguage(AppLanguage lang)
        {
            if (_language != lang)
            {
                _language = lang;
                LanguageChanged?.Invoke();
            }
        }

        private static readonly Dictionary<string, string> _viToEn = new Dictionary<string, string>
        {
            {"?? R?i Lobby", "?? Leave Lobby"},
            {"?? M?i b?n bè", "?? Invite friends"},
            {"?? B?T ??U GAME", "?? START GAME"},
            {"?ang ch? ng??i ch?i...", "Waiting for players..."},
            {"?? Chat s?nh ch?", "?? Lobby chat"},
            {"?? Ng??i ch?i trong phòng", "?? Players in room"},
            {"?? ??NG XU?T", "?? LOG OUT"},
            {"?? T?O PHÒNG M?I", "?? CREATE ROOM"},
            {"?? THAM GIA", "?? JOIN"},
            {"Nh?p IP phòng:", "Enter room IP:"},
        };

        public static void LocalizeContainer(Control root)
        {
            if (root == null) return;
            Action<Control> apply = null;
            apply = (ctrl) =>
            {
                if (!string.IsNullOrEmpty(ctrl.Text))
                {
                    if (_language == AppLanguage.English && _viToEn.TryGetValue(ctrl.Text, out var en))
                    {
                        ctrl.Text = en;
                    }
                    else if (_language == AppLanguage.Vietnamese && _viToEn.ContainsValue(ctrl.Text))
                    {
                        var vi = _viToEn.FirstOrDefault(kv => kv.Value == ctrl.Text).Key;
                        if (!string.IsNullOrEmpty(vi)) ctrl.Text = vi;
                    }
                }
                foreach (Control child in ctrl.Controls) apply(child);
            };
            apply(root);
        }
    }
}
