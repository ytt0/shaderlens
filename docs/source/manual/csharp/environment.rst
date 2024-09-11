Environment
===========

Shaderlens C# environment contains definitions for GLSL built in types and method, allowing C# shaders to build correctly.
The implementation was left empty, so it could not be used to test shader execution behavior.
The environment source files can be found `here <https://github.com/ytt0/shaderlens/src/Shaderlens.Environment>`_.

The environment files are included with the application, and can be imported to C# projects using the targets file at :file:`Resources\\CSharpEnvironment\\Build\\Environment.targets`.

The environment contains the following types and methods:

    - ``vec#``, ``ivec#``, ``uvec#``, ``bvec#``, ``mat#`` types, and static constructor methods.
    - ``sampler*`` types, and ``texture*(sampler*, ...)`` static methods.
    - Math static methods such as ``sin``, ``abs``, ``step``, ``mix``, etc.
    - A ``Float`` type, with ``_float``, and ``float_`` aliases, to implicitly convert between :csharp:`double` literals and  :csharp:`float` primitives (see floating point :ref:`workarounds<csharp-workaround-floating-point>` for usage examples).
    - :doc:`/appendix/built-in-uniforms` static imports.

The environment contains ``global using`` declarations, for importing types, and static methods.

The environment also contains a build task for automatically generating a suppressions file with rules for :csharp:`mainImage` methods usage, and uniforms modifiers suggestions (a project build is required for generating and updating these rules).

The environment targets file is imported by projects that are generated with C# templates (in the :ref:`New Project<features-new-project>` dialog), and can also be imported manually by adding the following code to the :file:`.csproj` file:

.. code-block:: xml

  <PropertyGroup>
    <EnvironmentPath>...\path\to\Shaderlens\Resources\CSharpEnvironment</EnvironmentPath>
  </PropertyGroup>

  <Import Project="$(EnvironmentPath)\Build\Environment.targets" />