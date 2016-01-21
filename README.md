# Localized Help Page
Localized Help Page adds localization to your ASP.NET WebAPI Help Page and automatically generates localized Help Page content.
Please note that this project is not affiliated with Microsoft in any way.

### Installation
Localized Help Page is delivered as a NuGet package and can be installed as folows:

    Install-Package Microsoft.AspNet.WebApi.LocalizedHelpPage

During the installation you will be prompted to overwrite some files from default Help Page. Press "Yes to all" button.

### How it works
Localized Help Page replaces all exact strings with a call to resource file. English, Russian and German languages are currently being supported. To support more languages, simply add a new resource file to `Areas\HelpPage\Resources\`. You also can help developing the project by offering translation to another languages.

Localized Help Page can also generate content from XML documentation for of your project. To make this feature work, you should write localized XML comments to your classes by adding attribute `xml:lang="xx-XX"` (`xx-XX` stands for culture name e.g. `ru-RU`) to standard notation as follows:

    /// <summary>Gets a value</summary>
    /// <param name="id">ID of the value</param>
    /// <returns>Value</returns>
    /// <summary xml:lang="ru-RU">Возвращает значение</summary>
    /// <param name="id" xml:lang="ru-RU">ID значения</param>
    /// <returns xml:lang="ru-RU">Значение</returns>
    public string Get(int id)
    {
        return "value";
    }

You can find a complete example on how to write localized XML comments in `Controllers\LocalizedValuesController.cs` which is installed along with LocalizedHelpPage package.

To enable localied help contents, edit `Areas\HelpPage\App_Start\HelpPageConfig.cs`. Uncomment line 38:

    config.SetDocumentationProvider(new LocalizedXmlDocumentationProvider(HttpContext.Current.Server.MapPath("~/App_Data/XmlDocument.xml")));
    
`~/App_Data/XmlDocument.xml` is the path to your XML documentation. Make sure to turn on "XML documentation file" option when building your project.
You also can use `MultipleSourceDocumentationProvider` to gather documentation from multiple sources:
    
    config.SetDocumentationProvider(new MultipleSourceDocumentationProvider(
        new LocalizedXmlDocumentationProvider(HttpContext.Current.Server.MapPath("~/App_Data/XmlDocument1.xml")),
        new LocalizedXmlDocumentationProvider(HttpContext.Current.Server.MapPath("~/App_Data/XmlDocument2.xml"))));

### Versions
#### 1.0.2
 * Multiple source documentation provider added
 * HelpController fixed to support localized Resource Model help

#### 1.0.1
 * German language added
 * Made some fixes for Russian language
 * HelpController fixed to work with T4MVC

#### 1.0.0
 * LocalizedHelpPage created with support of English and Russian languages

Enjoy it!