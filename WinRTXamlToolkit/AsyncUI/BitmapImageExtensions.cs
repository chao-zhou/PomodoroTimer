using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;

namespace WinRTXamlToolkit.AsyncUI
{
    public static class BitmapImageExtensions
    {
        public async static Task<ExceptionRoutedEventArgs> WaitForLoadedAsync(this BitmapImage bitmapImage)
        {
            var tcs = new TaskCompletionSource<ExceptionRoutedEventArgs>();

            // TODO: NOTE: This returns immediately if the image is already loaded,
            // but if the image already failed to load - the task will never complete and the app might hang.
            if (bitmapImage.PixelWidth > 0 ||
                bitmapImage.PixelHeight > 0)
            {
                tcs.SetResult(null);
                return await tcs.Task;
            }
                
            // Need to set it to null so that the compiler does not
            // complain about use of unassigned local variable.
            RoutedEventHandler reh = null;
            ExceptionRoutedEventHandler ereh = null;

            reh = (s, e) =>
            {
                bitmapImage.ImageOpened -= reh;
                bitmapImage.ImageFailed -= ereh;
                tcs.SetResult(null);
            };

            ereh = (s, e) =>
            {
                bitmapImage.ImageOpened -= reh;
                bitmapImage.ImageFailed -= ereh;
                tcs.SetResult(e);
            };

            bitmapImage.ImageOpened += reh;
            bitmapImage.ImageFailed += ereh;

            return await tcs.Task; 
        }
    }
}
