namespace Notifications.Domain.Notifications;

public static class NotificationTemplates
{
    public static class Security
    {
        public const string OtpVerification = "Security.OtpVerification";
        public const string Welcome = "Security.Welcome";
        public const string PasswordReset = "Security.PasswordReset";
        public const string EmailVerification = "Security.EmailVerification";
        public const string PasskeyAlert = "Security.PasskeyAlert";
    }

    public static class Academic
    {
        public const string CourseRegistration = "Academic.CourseRegistration";
        public const string GradeAnnounced = "Academic.GradeAnnounced";
    }

    public static class Common
    {
        public const string Transactional = "Common.Transactional";
    }
}
