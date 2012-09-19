using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace PomodoroTimer.Utils
{
    class NotificationManager
    {
        public ToastNotification CreateToast(string msg)
        {
            const ToastTemplateType tmp = ToastTemplateType.ToastText01;
            var xml = ToastNotificationManager.GetTemplateContent(tmp);

            SetToastText(xml,msg);
            //SetToastImage(xml);
            SetToastDuration(xml);
            SetToatAudio(xml);

            return new ToastNotification(xml);
        }

        public void ShowToast(string msg)
        {
            var toast = CreateToast(msg);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        public void ShowToast(ToastNotification toast)
        {
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }

        private static void SetToastText(XmlDocument xml,string msg)
        {
            var nodes = xml.GetElementsByTagName("text");
            nodes[0].AppendChild(xml.CreateTextNode(msg));
        }

/*
        private void SetToastImage(XmlDocument xml)
        {
            var nodes = xml.GetElementsByTagName("image");
            ((XmlElement)nodes[0]).SetAttribute("src", "ms-appx:///Assets/PomodoroTimerLogo.png");
            ((XmlElement)nodes[0]).SetAttribute("alt", "Pomodoro Timer");
        }
*/

        private static void SetToastDuration(XmlDocument xml)
        {
            var node = xml.SelectSingleNode("/toast");
            ((XmlElement)node).SetAttribute("duration", "long");
        }

        private static void SetToatAudio(XmlDocument xml)
        {
            var node = xml.SelectSingleNode("/toast");
            var audio = xml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.IM");
            node.AppendChild(audio);
        }
    }
}
