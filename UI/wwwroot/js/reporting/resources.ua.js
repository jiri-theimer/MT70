﻿(function (trv, $) {
    "use strict";

    var sr = {
        //warning and error string resources
        controllerNotInitialized: 'Контролер не ініціалізований.',
        noReportInstance: 'Немає екземпляру звіту.',
        missingTemplate: '! застарілий ресурс!',
        noReport: 'Немає звіту.',
        noReportDocument: 'Немає звітного документа.',
        missingOrInvalidParameter: 'Відсутнє або недійсне значення параметра. Введіть дійсні дані для всіх параметрів.',
        invalidParameter: 'Введіть дійсне значення.',
        invalidDateTimeValue: 'Введіть дійсну дату.',
        parameterIsEmpty: 'Значення параметра не може бути порожнім.',
        cannotValidateType: 'Не вдається перевірити параметр типу {type}.',
        loadingFormats: 'Завантаження ...',
        loadingReport: 'Завантаження звіту ...',
        preparingDownload: 'Підготовка документа до завантаження. Зачекайте, будь ласка ...',
        preparingPrint: 'Підготовка документа до друку. Будь ласка, почекайте ...',
        errorLoadingTemplates: "Помилка завантаження шаблонів засобу перегляду звітів. (templateUrl = '{0}').",
        errorServiceUrl: "Не вдається отримати доступ до служби звітування REST. (serviceUrl = '{0}'). Переконайтесь, що адреса служби є правильною, і за потреби увімкніть CORS. (https://enable-cors.org)",
        loadingReportPagesInProgress: 'Наразі завантажено сторінок: {0} ...',
        loadedReportPagesComplete: 'Готово. Загальна кількість завантажених сторінок: {0}.',
        noPageToDisplay: "Жодної сторінки для відображення.",
        errorDeletingReportInstance: "Помилка видалення екземпляру звіту: '{0}'.",
        errorRegisteringViewer: 'Помилка реєстрації засобу перегляду у службі.',
        noServiceClient: 'Для цього контролера не вказано жодного serviceClient.',
        errorRegisteringClientInstance: 'Помилка реєстрації екземпляра клієнта.',
        errorCreatingReportInstance: "Помилка створення екземпляра звіту (Report = '{0}').",
        errorCreatingReportDocument: "Помилка створення документа звіту (Report = '{0}'; Format = '{1}').",
        unableToGetReportParameters: 'Не вдалося отримати параметри звіту.',
        errorObtainingAuthenticationToken: 'Помилка отримання маркера автентифікації.',
        clientExpired: "Клацніть «Оновити», щоб відновити сеанс клієнта.",

        //viewer template string resources
        parameterEditorSelectNone: 'чіткий вибір',
        parameterEditorSelectAll: 'вибрати все',
        parametersAreaPreviewButton: 'Попередній перегляд',

        menuNavigateBackwardText: 'Перейдіть назад',
        menuNavigateBackwardTitle: 'Перейдіть назад',
        menuNavigateForwardText: 'Перейдіть вперед',
        menuNavigateForwardTitle: 'Перейдіть вперед',
        menuRefreshText: 'Оновити',
        menuRefreshTitle: 'Оновити',
        menuFirstPageText: 'Перша сторінка',
        menuFirstPageTitle: 'Перша сторінка',
        menuLastPageText: 'Остання сторінка',
        menuLastPageTitle: 'Остання сторінка',
        menuPreviousPageTitle: 'Попередня сторінка',
        menuNextPageTitle: 'Наступна сторінка',
        menuPageNumberTitle: 'Вибір номера сторінки',
        menuDocumentMapTitle: 'Переключити карту документів',
        menuParametersAreaTitle: 'Переключити область параметрів',
        menuZoomInTitle: 'Приближувати',
        menuZoomOutTitle: 'Зменшити',
        menuPageStateTitle: 'Переключити повну сторінку / ширину сторінки',
        menuPrintText: 'Друк...',
        menuContinuousScrollText: 'Переключити безперервну прокрутку',
        menuSendMailText: 'Надіслати електронне повідомлення',
        menuPrintTitle: 'Друк',
        menuContinuousScrollTitle: 'Переключити безперервну прокрутку',
        menuSendMailTitle: 'Надіслати електронне повідомлення',
        menuExportText: 'Експорт',
        menuExportTitle: 'Експорт',
        menuPrintPreviewText: 'PřПереключити попередній перегляд друку',
        menuPrintPreviewTitle: 'Переключити попередній перегляд друку',
        menuSearchText: 'Знайдіть',
        menuSearchTitle: 'Переключіться на [Знайти]',
        menuSideMenuTitle: 'Перемкнути бічне меню',

        sendEmailFromLabel: "From:",
        sendEmailToLabel: "To:",
        sendEmailCCLabel: "CC:",
        sendEmailSubjectLabel: "Subject:",
        sendEmailFormatLabel: "Format:",
        sendEmailSendLabel: "Send",
        sendEmailCancelLabel: "Cancel",

        //accessibility string resources
        ariaLabelPageNumberSelector: "Вибір номера сторінки. Показано сторінку {0} з {1}.",
        ariaLabelPageNumberEditor: "Page number editor",
        ariaLabelExpandable: "Expandable",
        ariaLabelSelected: "Selected",
        ariaLabelParameter: "parameter",
        ariaLabelErrorMessage: "Error message",
        ariaLabelParameterInfo: "Contains {0} options",
        ariaLabelMultiSelect: "Multiselect",
        ariaLabelMultiValue: "Multivalue",
        ariaLabelSingleValue: "Single value",
        ariaLabelParameterDateTime: "DateTime",
        ariaLabelParameterString: "String",
        ariaLabelParameterNumerical: "Numerical",
        ariaLabelParameterBoolean: "Boolean",
        ariaLabelParametersAreaPreviewButton: 'Preview the report',
        ariaLabelMainMenu: 'Main menu',
        ariaLabelCompactMenu: 'Compact menu',
        ariaLabelSideMenu: 'Side menu',
        ariaLabelDocumentMap: 'Document map area',
        ariaLabelDocumentMapSplitter: 'Document map area splitbar.',
        ariaLabelParametersAreaSplitter: 'Parameters area splitbar.',
        ariaLabelPagesArea: 'Report contents area',
        ariaLabelSearchDialogArea: 'Search area',
        ariaLabelSendEmailDialogArea: "Send email area",
        ariaLabelSearchDialogStop: 'Stop search',
        ariaLabelSearchDialogOptions: 'Search options',
        ariaLabelSearchDialogNavigateUp: 'Navigate up',
        ariaLabelSearchDialogNavigateDown: 'Navigate down',
        ariaLabelSearchDialogMatchCase: 'Match case',
        ariaLabelSearchDialogMatchWholeWord: 'Match whole word',
        ariaLabelSearchDialogUseRegex: 'Use regex',
        ariaLabelMenuNavigateBackward: 'Navigate backward',
        ariaLabelMenuNavigateForward: 'Navigate forward',
        ariaLabelMenuRefresh: 'Оновитиt',
        ariaLabelMenuFirstPage: 'Перша сторінка',
        ariaLabelMenuLastPage: 'Остання сторінка',
        ariaLabelMenuPreviousPage: 'Попередня сторінка',
        ariaLabelMenuNextPage: 'Наступна сторінка',
        ariaLabelMenuPageNumber: 'Вибір номера сторінки',
        ariaLabelMenuDocumentMap: 'Toggle document map',
        ariaLabelMenuParametersArea: 'Toggle parameters area',
        ariaLabelMenuZoomIn: 'Zoom in',
        ariaLabelMenuZoomOut: 'Zoom out',
        ariaLabelMenuPageState: 'Toggle FullPage/PageWidth',
        ariaLabelMenuPrint: 'Print',
        ariaLabelMenuContinuousScroll: 'Continuous scrolling',
        ariaLabelMenuSendMail: 'Send an email',
        ariaLabelMenuExport: 'Export',
        ariaLabelMenuPrintPreview: 'Toggle print preview',
        ariaLabelMenuSearch: 'Search in report contents',
        ariaLabelMenuSideMenu: 'Toggle side menu',
        ariaLabelSendEmailFrom: "From email address",
        ariaLabelSendEmailTo: "Recipient email address",
        ariaLabelSendEmailCC: "Carbon Copy email address",
        ariaLabelSendEmailSubject: "Email subject:",
        ariaLabelSendEmailFormat: "Report format:",
        ariaLabelSendEmailSend: "Send email",
        ariaLabelSendEmailCancel: "Cancel sending email",

        //search dialog resources
        searchDialogTitle: 'Шукати у змісті звіту',
        searchDialogSearchInProgress: "пошук ...",
        searchDialogNoResultsLabel: "Немає результатів",
        searchDialogResultsFormatLabel: "Результат {0} з {1}",
        searchDialogStopTitle: 'Stop Search',
        searchDialogNavigateUpTitle: 'Navigate Up',
        searchDialogNavigateDownTitle: 'Navigate Down',
        searchDialogMatchCaseTitle: 'Match Case',
        searchDialogMatchWholeWordTitle: 'Match Whole Word',
        searchDialogUseRegexTitle: 'Use Regex',
        searchDialogCaptionText: 'Знайдіть',
        searchDialogPageText: 'сторінка',

        // Send Email dialog resources

        sendEmailDialogTitle: "Прати имейл",
        sendEmailValidationEmailRequired: "Email field is required",
        sendEmailValidationEmailFormat: "Email format is not valid",
        sendEmailValidationSingleEmail: "The field accepts a single email address only",
        sendEmailValidationFormatRequired: "Format field is required",
        errorSendingDocument: "Error sending report document (Report = '{0}').",
    };
    trv.sr = $.extend(trv.sr, sr);
}(window.telerikReportViewer = window.telerikReportViewer || {}, jQuery));
