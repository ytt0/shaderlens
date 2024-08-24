Environment
===========

Shaderlens C# environment contains definitions for GLSL built in types and method, allowing C# shaders to build correctly.
The implementation was left empty, so it could not be used to test shader execution behavior.
The source files can be found `here <https://github.com/ytt0/shaderlens/src/Shaderlens.Environment>`_.

The environment files are included as resources with the application, and can be linked directly from the application folder, at :file:`Resources\\CSharpEnvironment`.

The environment contains the following types and methods:

    - ``vec#``, ``ivec#``, ``uvec#``, ``bvec#``, ``mat#`` types, and static constructor methods.
    - ``sampler*`` types, and ``texture*(sampler*, ...)`` static methods.
    - Math static methods such as ``sin``, ``abs``, ``step``, ``mix``, etc.
    - A ``Float`` type, with ``_float``, and ``float_`` aliases, to implicitly convert between :csharp:`double` literals and  :csharp:`float` primitives (see floating point :ref:`workarounds<csharp-workaround-floating-point>` for usage examples).
    - :doc:`/appendix/built-in-uniforms` static imports.

The environment also contains ``global using`` declarations, for importing types, and static methods.

The environment source files are linked automatically, as hidden items, to projects that are generated with the C# project templates in the :ref:`New Project<features-new-project>` dialog, and can also be linked manually by adding the following code to the :file:`.csproj` file:

.. code-block:: xml

    <ItemGroup>
      <Compile Include="...\path\to\Shaderlens\Resources\CSharpEnvironment\*.cs">
        <Link>Environment\%(RecursiveDir)%(Filename)%(Extension)</Link>
        <Visible>false</Visible>
      </Compile>
    </ItemGroup>
