using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using MyToolkit.Networking;

namespace SamplePhoneApp.Views
{
	public partial class SamplePage
	{
		public SamplePage()
		{
			InitializeComponent();

			Html.Html = @"<tr class=""message""><td class=""message"">This one will take a bit of time and manipulation. In order to unlock the achievement, you will need to create a 9-letter word that will span across both '<b>DW</b>' bonuses <u>at the same time</u>. You can do this in a local '<i>Pass & Play</i>' game.<br/><br/>...so choose a word based on your starting letters from here:<br/><a href=""http://www.becomeawordgameexpert.com/wordlists9.htm"" rel=""external nofollow"">http://www.becomeawordgameexpert.com/wordlists9.htm</a><br/><br/>You'll have to abuse the swap-tile feature, but if you keep doing that, the game will start giving you back the tiles you first swapped out. Just use what you don't want on another part of the board and hold or build part of the word you've selected, with the other tiles.<br/><br/>i.e,<br/><span class=""quote"">I used the word '<b>RETENTION</b>' spanning from the upper-right '<b>DW</b>' to the lower-right '<b>DW</b>'<br/><br/>- I started with a word that ended with an '<b>N</b>'. The '<b>N</b>' was positioned between the two '<b>DW</b>'s<br/>- Next, I built my letter up to the word '<b>EN</b>'<br/>- After a few turns, to '<b>TEN</b>'<br/>- A bit more tile-swapping and built it up to '<b>TENT</b>'<br/>- Now held the remaining tiles needed until I could complete the entire word 'RE<b>TENT</b>ION'</span>Just remember. You have to use both '<b>DW</b>' spaces at the same time. <br/><br/>Good luck.<br/><br/><br/><u>Edit 04.29.13</u>: Here's some visual aid.<br/><br/><div class=""externalimage"">
<img src=""http://i39.tinypic.com/3328m5u.png"" class=""externalimage"" alt=""External image"" title=""External image""/></div><br/><div class=""externalimage"">
<img src=""http://i41.tinypic.com/30tpcm1.png"" class=""externalimage"" alt=""External image"" title=""External image""/></div><br/><div class=""externalimage"">
<img src=""http://i40.tinypic.com/4kvi9x.png"" class=""externalimage"" alt=""External image"" title=""External image""/></div><br/><div class=""externalimage"">
<img src=""http://i44.tinypic.com/2n810x.png"" class=""externalimage"" alt=""External image"" title=""External image""/></div><br/><div class=""externalimage"">
<img src=""http://i43.tinypic.com/53l8pc.png"" class=""externalimage"" alt=""External image"" title=""External image""/></div></td>
</tr>";
		}

		private async void OnDownload(object sender, RoutedEventArgs e)
		{
			var prog = new Progress<HttpProgress>();
			prog.ProgressChanged += (o, p) =>
			{
				Progress.Minimum = 0;
				Progress.Value = p.ReadBytes;
				Progress.Maximum = p.TotalBytes;
			};

			var request = new HttpGetRequest(new Uri("http://ipv4.download.thinkbroadband.com/10MB.zip", UriKind.Absolute));
			request.UseCache = false; 

			var response = await Http.GetAsync(request, CancellationToken.None, prog);
		}

	    private void OnSendImage(object sender, RoutedEventArgs e)
	    {
            var task = new PhotoChooserTask();
            task.ShowCamera = true;
            task.Completed += async (s, r) =>
            {
                if (r.TaskResult == TaskResult.OK)
                {
                    try
                    {
                        var request = new HttpPostRequest("http://yourdomain.com/Upload");
                        request.ContentType = "multipart/form-data";
                        request.Data.Add("vve", "VvE Naam");                                
                        request.Files.Add(new HttpPostFile("image", "image", r.ChosenPhoto)); 
                        
                        //// TODO: Add pr
                        var response = await Http.PostAsync(request);
                        MessageBox.Show(response.Response);
                    }
                    catch (Exception exception)
                    {
                        MessageBox.Show(exception.Message);
                    }
                }
            };
            task.Show();
	    }
	}
}