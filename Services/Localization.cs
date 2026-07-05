namespace EasyBinds.Services;

public static class L10n
{
    public static string Lang { get; set; } = "en";

    private static readonly Dictionary<string, Dictionary<string, string>> Data = new()
    {
        ["en"] = new()
        {
            ["app_title"] = "EasyBinds — Global Hotkey Manager",
            ["add_binding"] = "Add Binding",
            ["edit"] = "Edit",
            ["delete"] = "Delete",
            ["enable_all"] = "Enable All",
            ["disable_all"] = "Disable All",
            ["run_at_startup"] = "Run at startup",
            ["ready"] = "Ready",
            ["loaded_bindings"] = "Loaded {0} binding(s)",
            ["added"] = "Added: {0}",
            ["edited"] = "Edited: {0}",
            ["deleted"] = "Binding deleted",
            ["select_to_edit"] = "Select a binding to edit",
            ["select_to_delete"] = "Select a binding to delete",
            ["confirm_delete"] = "Delete binding \"{0}\"?",
            ["confirm"] = "Confirm",
            ["all_enabled"] = "All bindings enabled",
            ["all_disabled"] = "All bindings disabled",
            ["executed"] = "Executed: {0} → {1}",
            ["theme"] = "Theme",
            ["dark"] = "Dark",
            ["light"] = "Light",
            ["language"] = "Language",

            // Add binding window
            ["add_binding_title"] = "Add Binding",
            ["edit_binding_title"] = "Edit Binding",
            ["press_keys"] = "Press the key combination:",
            ["action_type"] = "Action type:",
            ["launch_program"] = "Launch Program",
            ["close_window"] = "Close Active Window",
            ["run_command"] = "Run Command",
            ["open_website"] = "Open Website",
            ["show_message"] = "Show Message",
            ["toggle_mute"] = "Toggle Mute",
            ["maximize_window"] = "Maximize Window",
            ["minimize_window"] = "Minimize Window",
            ["show_desktop"] = "Show Desktop",
            ["volume_up"] = "Volume Up",
            ["volume_down"] = "Volume Down",
            ["program_path"] = "Program path:",
            ["no_param"] = "(no parameter needed)",
            ["command"] = "Command:",
            ["website_url"] = "Website URL:",
            ["message_text"] = "Message text:",
            ["browse"] = "Browse...",
            ["choose"] = "Choose...",
            ["file_label"] = "File:",
            ["ok"] = "OK",
            ["cancel"] = "Cancel",
            ["err_no_keys"] = "Please press a key combination.",
            ["err_no_param"] = "Please enter a value for the parameter.",
            ["select_program"] = "Select Program",
            ["exec_filter"] = "Executables (*.exe)|*.exe|All files (*.*)|*.*",

            // Header
            ["enabled"] = "Enabled",
            ["hotkey"] = "Hotkey",
            ["action"] = "Action",
            ["parameter"] = "Parameter",

            // Tray
            ["tray_text"] = "EasyBinds\nDouble-click to open",
            ["tray_show"] = "Show",
            ["tray_exit"] = "Exit",

            // Errors
            ["launch_error"] = "Failed to launch:\n{0}\n\n{1}",
            ["easybinds_error"] = "EasyBinds Error",
            ["empty_list"] = "No bindings yet. Click '{0}' to create one.",

            // Admin rights
            ["request_admin"] = "Request Admin",
            ["why_admin"] = "Why?",
            ["admin_title"] = "Admin Rights",
            ["admin_msg"] = "EasyBinds works without admin rights for most actions.\n\n" +
                "You need admin rights only if:\n" +
                "  - Closing programs that run as Administrator\n" +
                "  - Launching programs that require elevation\n" +
                "  - Running commands that modify system settings\n\n" +
                "Click 'Request Admin' to restart with elevated privileges.",
            ["admin_restart"] = "Restarting as admin...",
        },

        ["ru"] = new()
        {
            ["app_title"] = "EasyBinds — Менеджер горячих клавиш",
            ["add_binding"] = "Добавить",
            ["edit"] = "Изменить",
            ["delete"] = "Удалить",
            ["enable_all"] = "Вкл. все",
            ["disable_all"] = "Выкл. все",
            ["run_at_startup"] = "Автозагрузка",
            ["ready"] = "Готово",
            ["loaded_bindings"] = "Загружено: {0}",
            ["added"] = "Добавлено: {0}",
            ["edited"] = "Изменено: {0}",
            ["deleted"] = "Бинд удалён",
            ["select_to_edit"] = "Выберите бинд для изменения",
            ["select_to_delete"] = "Выберите бинд для удаления",
            ["confirm_delete"] = "Удалить бинд \"{0}\"?",
            ["confirm"] = "Подтверждение",
            ["all_enabled"] = "Все бинды включены",
            ["all_disabled"] = "Все бинды отключены",
            ["executed"] = "Выполнен: {0} → {1}",
            ["theme"] = "Тема",
            ["dark"] = "Тёмная",
            ["light"] = "Светлая",
            ["language"] = "Язык",

            ["add_binding_title"] = "Добавить бинд",
            ["edit_binding_title"] = "Изменить бинд",
            ["press_keys"] = "Нажмите комбинацию клавиш:",
            ["action_type"] = "Тип действия:",
            ["launch_program"] = "Запуск программы",
            ["close_window"] = "Закрыть окно",
            ["run_command"] = "Выполнить команду",
            ["open_website"] = "Открыть сайт",
            ["show_message"] = "Показать сообщение",
            ["toggle_mute"] = "Откл. звук",
            ["maximize_window"] = "Развернуть окно",
            ["minimize_window"] = "Свернуть окно",
            ["show_desktop"] = "Показать рабочий стол",
            ["volume_up"] = "Увеличить громкость",
            ["volume_down"] = "Уменьшить громкость",
            ["program_path"] = "Путь к программе:",
            ["no_param"] = "(параметр не нужен)",
            ["command"] = "Команда:",
            ["website_url"] = "URL сайта:",
            ["message_text"] = "Текст сообщения:",
            ["browse"] = "Обзор...",
            ["choose"] = "Выбрать...",
            ["file_label"] = "Файл:",
            ["ok"] = "OK",
            ["cancel"] = "Отмена",
            ["err_no_keys"] = "Пожалуйста, нажмите комбинацию клавиш.",
            ["err_no_param"] = "Пожалуйста, введите значение параметра.",
            ["select_program"] = "Выберите программу",
            ["exec_filter"] = "Программы (*.exe)|*.exe|Все файлы (*.*)|*.*",

            ["enabled"] = "Вкл",
            ["hotkey"] = "Горячая клавиша",
            ["action"] = "Действие",
            ["parameter"] = "Параметр",

            ["tray_text"] = "EasyBinds\nДвойной клик — открыть",
            ["tray_show"] = "Показать",
            ["tray_exit"] = "Выход",

            ["launch_error"] = "Не удалось запустить:\n{0}\n\n{1}",
            ["easybinds_error"] = "Ошибка EasyBinds",
            ["empty_list"] = "Биндов нет. Нажмите «{0}», чтобы создать.",

            // Admin rights
            ["request_admin"] = "Запросить права",
            ["why_admin"] = "Зачем?",
            ["admin_title"] = "Права администратора",
            ["admin_msg"] = "EasyBinds работает без прав администратора для большинства действий.\n\n" +
                "Права администратора нужны только если:\n" +
                "  - Закрытие программ, запущенных от имени администратора\n" +
                "  - Запуск программ, требующих повышение привилегий\n" +
                "  - Выполнение команд, изменяющих системные настройки\n\n" +
                "Нажмите «Запросить права» для перезапуска с повышением.",
            ["admin_restart"] = "Перезапуск от администратора...",
        }
    };

    public static string _(string key) =>
        Data.TryGetValue(Lang, out var dict) && dict.TryGetValue(key, out var val) ? val : key;
}
