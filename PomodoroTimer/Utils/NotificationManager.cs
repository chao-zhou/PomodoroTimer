using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;


namespace PomodoroTimer
{
    class NotificationManager
    {
        public ToastNotification CreateToast(string msg)
        {
            var tmp = ToastTemplateType.ToastText01;
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

        private void SetToastText(XmlDocument xml,string msg)
        {
            var nodes = xml.GetElementsByTagName("text");
            nodes[0].AppendChild(xml.CreateTextNode(msg));
        }

        private void SetToastImage(XmlDocument xml)
        {
            var nodes = xml.GetElementsByTagName("image");
            ((XmlElement)nodes[0]).SetAttribute("src", "ms-appx:///Assets/PomodoroTimerLogo.png");
            ((XmlElement)nodes[0]).SetAttribute("alt", "Pomodoro Timer");
        }

        private void SetToastDuration(XmlDocument xml)
        {
            var node = xml.SelectSingleNode("/toast");
            ((XmlElement)node).SetAttribute("duration", "long");
        }

        private void SetToatAudio(XmlDocument xml)
        {
            var node = xml.SelectSingleNode("/toast");
            var audio = xml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.IM");
            node.AppendChild(audio);
        }
    }
}
