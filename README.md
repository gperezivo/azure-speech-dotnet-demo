# azure-speech-dotnet-demo

## Info

La solución está hecha con [Visual Studio 2022 Preview](https://visualstudio.microsoft.com/es/vs/preview/) y usa [.NET 6](https://dotnet.microsoft.com/download/dotnet/6.0) con su versión actual (RC-1)

Para cambiar la versión de .NET cambiar en el fichero [Directory.Build.props](./Directory.Build.props) el valor de __TargetFrameworkVersion__ por la versión deseada
[Supported Target Framework](https://docs.microsoft.com/en-us/dotnet/standard/frameworks#supported-target-frameworks)
```xml
<Project>
	<PropertyGroup Label="Project versions">
		<TargetFrameworkVersion>net6.0</TargetFrameworkVersion>
		<LangVersion>latest</LangVersion>
	</PropertyGroup>
  ...
</Project>
```
Esto cambiará el framework para todos los proyectos del repositorio.

### ADVERTENCIA ###
> Los proyectos están usando funcionalidades de __.NET 6__
> 
> Si cambias el target framework deberás de modificar el [Program.cs](./speech-to-text/Program.cs) para incluir la definición del namespace
> y el típico static async Task Main(string[] args). 
> 
> Tendrás que poner todo el código que hay dentro de este método quedando algo así:

```csharp
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace SpeechToText
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            
         	//Inserta aquí todo el código actual de Program.cs
        }
    }
}
```

## Speech to text
En el ejemplo se utiliza la transcripción continua por eventos.

Captura el audio del microfono por defecto y lo transcribe.

En el fichero [appsettings.json](./speech-to-text/appsettings.json) debemos configurar los siguientes valores:

```json

{
  "SpeechService": {
    "Endpoint": "",
    "Key": "",
    "Region": "northeurope",
    "ToLanguage": "es-ES"
  }
}
```
| Clave | Valor | 
|-------|-------|
| Endopoint | El endpoint de nuestro recurso |
| Key | Api Key de nuestro recurso |
| Region | Localización de Azure donde está desplegado nuestro recurso |
| ToLanguage | El idioma en el que vamos a hablar |


Si no sabes como crear un Azure Cognitive Services o ya lo tienes creado y necesitas obtener el endpoint y la key, sigue estos enlaces:

_HOW TO: [Cognitive services - Create a new Cognitive Services resource](https://docs.microsoft.com/en-us/azure/cognitive-services/cognitive-services-apis-create-account?tabs=multiservice%2Clinux#create-a-new-azure-cognitive-services-resource)_

_HOW TO: [Cognitive services - Get the keys for your resource](https://docs.microsoft.com/en-us/azure/cognitive-services/cognitive-services-apis-create-account?tabs=multiservice%2Clinux#get-the-keys-for-your-resource)_

## Speech translator
Incoming

