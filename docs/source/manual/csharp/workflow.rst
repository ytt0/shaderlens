Workflow
========

To work with Visual Studio, a C# *class library* can be used, with a class that contains a :csharp:`mainImage` implementation for each pass. A common code could also be added as a base class for each pass, or as a static class referenced with a :csharp:`using static` directive. The gap between GLSL built-in types and methods, and C# types, could be filled by including the :doc:`environment` files.

The viewer could load C# code and convert it to GLSL with the help of a few :ref:`built-in<conversion-rules-built-in>` and 
:ref:`language specific<conversion-rules-language>` conversion rules.
In addition to that, a few :ref:`line<conversion-rules-line>` and :ref:`text<conversion-rules-text>` conversion rules could also be applied in places where C# and GLSL are still not fully compatible.

The :ref:`New Project<features-new-project>` templates can be used for creating C# projects.
More C# projects examples can be found `here <https://github.com/ytt0/shaderlens/tree/main/examples>`_.


Project Structure Example
-------------------------

.. code-block:: csharp
    :caption: Class Structure - Image.cs

    namespace Shader1;

    class Image : Common // access common members
    {
        void mainImage(out vec4 fragColor, in vec2 fragCoord)
        {
            vec3 result = color; // access common uniform
            // ...

            result = linearToSrgb(result); // access common method
            fragColor = vec4(result, 1.0);
        }
    }

.. code-block:: csharp
    :caption: Class Structure - Common.cs

    namespace Shader1;

    // internal class, with public or protected members
    // accessible through inheritance
    class Common
    {
        //@uniform, type: linear-rgb
        protected vec3 color = vec3(0.5);

        protected vec3 linearToSrgb(vec3 color)
        {
            //@const
            float_ f = 1.0 / 2.2;
            return vec3(pow(color.r, f), pow(color.g, f), pow(color.b, f));
        }
    }

.. code-block:: json
    :caption: Project definition - Shader1.json

    {
        "Common": "Common.cs",
        "Image": {
            "Source": "Image.cs",
            "Channel0": "Image",
            "Channel1": "Keyboard"
        }
    }

.. code-block:: xml
    :caption: C# Project - Shader1.csproj

    <Project Sdk="Microsoft.NET.Sdk">

      <PropertyGroup>
        <TargetFramework>netstandard2.0</TargetFramework>
        <LangVersion>10.0</LangVersion>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>disable</Nullable>
        <EnvironmentPath>...\path\to\Shaderlens\Resources\CSharpEnvironment</EnvironmentPath>
      </PropertyGroup>

      <ItemGroup>
        <Compile Include="$(EnvironmentPath)\*.cs">
        <Link>Environment\%(RecursiveDir)%(Filename)%(Extension)</Link>
        <Visible>false</Visible>
        </Compile>
      </ItemGroup>

      <ItemGroup>
        <Compile Update="GlobalSuppressions.cs">
        <Visible>false</Visible>
        </Compile>
      </ItemGroup>

    </Project>

.. code-block:: text
    :caption: File Structure

    │   Shader1.sln                     -- Visual Studio solution
    │   Shader1.csproj                  -- A class library project
    │
    │   Shader1.json                    -- Project definition
    │
    │   Common.cs                       -- Common code
    │   Image.cs                        -- Image pass
    │
    │   GlobalSuppressions.cs (hidden)
    │
    ├───Environment (hidden)            -- Linked Environment files
    │       Float.cs
    │       Vec2.cs
    │       ...
    │       Suppressions.cs
    │       Uniforms.cs
    │       Usings.cs
    │
    └───Properties
        launchSettings.json             -- Enable Ctrl+F5 launch