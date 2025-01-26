<h1>World Cities</h1>
<p>WorldCities is a study project in which I implement the same project as 
  <a url="https://books.google.pt/books/about/ASP_NET_Core_8_and_Angular.html?id=4Ez3EAAAQBAJ&redir_esc=y">Valerio De Sanctis' book, ASP.NET Core 8 and Angular</a>. 
  However, it is not exactly the same because, in addition to using newer versions of Angular, TypeScript and Node.JS 
  compared to the book, I do some tests and go deeper into some topics.</p>

<h2>My WorldCities x book WorldCities</h2>
<h3>Server Side</h3>
<ul>
  <li><b>Spatial data:</b> I am using a Geography field in the MSSQL database to represent the city's location. 
    For mapping through the model, I use the Microsoft.EntityFrameworkCore.SqlServer.NetTopologySuite library. 
    The NetTopologySuite library replaces the DbGeography library used in .NET Framework.</li>
  <li><b>IQueryable Extensions:</b> In the book project, De Sanctis uses the System.Link.Dynamic.Core library with IQueryable extensions in the generic ApiResult<T> class to sort and filter the data. 
    I implemented my own extensions, creating overloads of the OrderBy and Where methods, so it was not necessary to import the Dynamic.Core library.</li>
</ul>
<h3>Client Side</h3>
<ul>
  <li><b>Data Source: </b>To load the city data into the cities component, instead of using the MatTableDataSource component, I created my own DataSource, 
    so the countries component uses the MatTableDataSource as in the book and the cities component has its own data source implemented in city-data-source.ts</li>
  <li><b>Validations: </b>In the city-edit component I included some extra validations on the form fields (server side validations are still missing).</li>
  <li><b>SCSS:</b> The project has been adapted to work with SCSS instead of CSS, but I haven't done much with the design yet.</li>
  <li><b>Latest Angular Version:</b> I am using Angular v19, which is the latest.</li>
</ul>
<p>Apart this, have a lot of others small things that I did in the project that sometimes they works like expected, sometimes no. 
   But like I said, this is a study project, to test .NET and Angular news and features.</p>
<h2>Blazor Client</h2>
<p>To make a comparison with Angular, a Blazor project was included in the solution. It's not just about compare, but also about trying and test things.</p>
<h3>Highlights</h3>
<ul>
  <li>Blazor.Bootstrap</li>
</ul>
