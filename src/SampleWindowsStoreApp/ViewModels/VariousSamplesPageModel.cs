using MyToolkit.MVVM;
using MyToolkit.Mvvm;

namespace SampleWindowsStoreApp.ViewModels
{
    public class VariousSamplesPageModel : ViewModelBase
    {
		public string Html { get; private set; }

        public string Test { get; set; }

		public VariousSamplesPageModel()
		{
		    Test = "Hello world";
            Html =
@"<h1>Heading 1</h1>
<h2>Heading 2</h2>
<p>
    Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore 
    et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. 
    Stet clita kasd gubergren.
</p>
<img src=""http://rsuter.com/Projects/MyToolkit/IconNuGet.png"" />
<p>
    Lorem ipsum <strong>dolor sit amet, consetetur sadipscing elitr, sed</strong> diam nonumy eirmod tempor invidunt ut labore 
    et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. 
    Stet clita kasd gubergren, no <a href=""http://rsuter.com"">sea takimata <strong>sanctus est Lorem ipsum dolor</strong> 
    sit amet. Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam</a> nonumy eirmod tempor invidunt ut 
    labore et dolore magna aliquyam erat, sed diam voluptua. <em>At vero eos et accusam et justo</em> duo dolores et ea 
    rebum. Stet <strong>clita kasd <a href=""#"">gubergren</a></strong>, no sea takimata sanctus est Lorem ipsum dolor sit amet.
</p>
<h2>Heading 2</h2>
<p>
    Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore 
    et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. 
    Stet clita kasd gubergren.
</p>
<ul>
    <li>Item1</li>
    <li>Item2</li>
    <li>Item3</li>
    <li>Item4</li>
</ul>
<p>
    Lorem ipsum dolor sit amet, consetetur sadipscing elitr, sed diam nonumy eirmod tempor invidunt ut labore 
    et dolore magna aliquyam erat, sed diam voluptua. At vero eos et accusam et justo duo dolores et ea rebum. 
    Stet clita kasd gubergren.
</p>";
		}
	}
}
