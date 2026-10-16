import type { TranslationKeys } from './tr';

export const ru: Record<TranslationKeys, string> = {
  // Верхняя панель и учреждение
  university_name: 'ТУРЕЦКАЯ РЕСПУБЛИКА АНАТОЛИЙСКИЙ ГОС. УНИВЕРСИТЕТ',
  system_title: 'Информационная система управления обучением (OBS Портал)',
  about_us: 'О нас',
  announcements: 'Объявления',

  // Центральная карточка входа
  login_card_title: 'Вход в систему',
  login_card_subtitle: 'Единая точка входа по учетным данным университета',
  edevlet_login: 'Вход через Госуслуги',
  obskimlik_login: 'Вход через obsID',
  or_credentials: 'или по учетным данным',
  email_or_tckn_label: 'Эл. почта или номер удостоверения',
  email_or_tckn_placeholder: 'имя.фамилия@obs.edu.tr или номер',
  email_required_error: 'Пожалуйста, введите имя пользователя или эл. почту.',
  password_label: 'Пароль',
  password_placeholder: '••••••••••••',
  password_required_error: 'Пожалуйста, введите пароль.',
  forgot_password: 'Забыли пароль?',
  back_to_login: 'Назад ко входу',
  forgot_password_title: 'Безопасный сброс пароля',
  forgot_password_subtitle: 'Установите новый пароль с подтверждением личности',
  remember_me: 'Оставаться в системе на этом устройстве',
  login_button: 'Войти в систему',
  login_success: 'Сессия подтверждена. Перенаправление...',
  login_failed: 'Ошибка входа! Пожалуйста, проверьте данные.',

  // Быстрый демо-вход
  demo_title: 'ДЕМО-АККАУНТЫ (Автозаполнение для тестирования):',
  demo_student: 'Студент',
  demo_academic: 'Преподаватель',
  demo_staff: 'Студенческий отдел',
  demo_multi_role: 'Несколько ролей (Ассистент и Аспирант)',

  // Нижний колонтитул
  footer_copyright: 'OBS Все права защищены © 2026',
  footer_kvkk: 'Политика конфиденциальности и защита данных',
  footer_phone_directory: 'Телефонный справочник',
  footer_website: 'Веб-сайт',
  footer_support: 'Поддержка OBS',
  footer_evaluation: 'Институциональная оценка',

  // Модальные окна
  modal_about_title: 'Об университете и системе OBS',
  modal_about_content: 'Информационная система студентов (OBS) предоставляет современную, безопасную цифровую среду кампуса, полностью соответствующую Болонскому процессу и стандартам высшего образования.',
  modal_accreditation: 'Аккредитация: Полная институциональная аккредитация YÖKAK (5 лет)',
  modal_security: 'Безопасность данных: ISO 27001 и защита персональных данных',
  modal_announcements_title: 'Актуальные университетские объявления',
  modal_phone_directory_title: 'Внутренний телефонный справочник университета',
  modal_support_title: 'Техническая поддержка OBS',
  modal_support_desc: 'По вопросам сброса пароля, регистрации на курсы и академических справок обращайтесь круглосуточно:',
  modal_support_email: 'Эл. почта: obs.destek@anadolu.edu.tr',
  modal_support_call: 'Колл-центр: 0850 300 06 27 (Будни 08:30 - 17:30)',
  modal_support_desk: 'Стойка обслуживания: Главный корпус, 1-й этаж',
  modal_kvkk_title: 'Защита персональных данных и правовые положения',
  modal_close: 'Закрыть',
  modal_understood: 'Понятно',
};
