Uniforms
========

Uniforms that are declared in the code, are automatically added to the :ref:`Uniforms view<features-uniforms-view>` where they can be viewed and edited. 
Available built-in uniforms are listed :doc:`here</appendix/built-in-uniforms>`.

Uniforms Declaration
--------------------

Uniforms are declared with the :glsl:`uniform` keyword, with an optional default value assignment, and a :glsl:`//@uniform` :doc:`Line Annotation <line-annotation>` with more properties to customize the view. |br|
Uniforms are global, and can be declared in multiple places, as long as the same type and default value (if not omitted) are used. To avoid duplication, the common sources could be used for shared uniforms declaration.

.. code-block:: glsl

    //@uniform, min: 0.0, max: 10.0, step: 0.1
    uniform float blurSize = 1.0;

.. _uniforms-properties:

Uniforms Properties
-------------------

To customize the way uniforms are being displayed and edited in the :ref:`Uniforms view<features-uniforms-view>`, the |br|
:glsl:`//@uniform` line annotation could be used with the following properties:

Common properties:
    :display-name: The name that is displayed in the :ref:`Uniforms view<features-uniforms-view>` (*string*), if not provided, the uniform name is displayed with a default formatting (for example, "blurRadius1", would be displayed as "Blur Radius 1").
    :index: Uniform :ref:`order<uniforms-ordering>` priority (*integer*).

Additional properties:
    For :glsl:`vec3`, :glsl:`vec4`:

    :type: Uniform underlying type, can be one of ``srgb``, or ``linear-rgb``. |br|
        When provided, a color picker is used for editing the value, with an alpha channel for :glsl:`vec4` uniforms. The type does not change the uniform value itself, only the way the color picker interprets it.
        Note that if ``linear-rgb`` type is used, the color has to be converted back to ``srgb`` by the shader, before it could be displayed, see an example below.

    For :glsl:`int`, :glsl:`float`, :glsl:`vec#`, :glsl:`ivec#`, :glsl:`uvec#`:

    :min / max: Value bounds (should be compatible with the uniform type).
    :step: Value increment size (should be compatible with the uniform type).

Example
    .. code-block:: glsl

        //@uniform, min: 0.0, max: vec2(5.0, 10.0), step: 0.1
        uniform vec2 boxBlurSize = vec2(1.0, 2.0);

        //@uniform, type: linear-rgb
        uniform vec4 backgroundColor = vec4(1.0, 0.5, 0.0, 1.0);

        void mainImage(out vec4 fragColor, in vec2 fragCoord)
        {
            fragColor.rgb = backgroundColor; // read linear-rgb

            // ...

            // convert to srgb before displaying
            fragColor.rgb = vec3(pow(fragColor.r, 1.0 / 2.2), pow(fragColor.g, 1.0 / 2.2), pow(fragColor.b, 1.0 / 2.2));
        }

.. _uniforms-group:

Uniforms Group
--------------

Uniforms groups could be declared using the :glsl:`//@uniform-group` line annotation, with the following properties:

:name:
    **(required)** Uniform group name (*string*). |br|
    When the same group name is declared in multiple sources, uniforms are merged to a single group in the :ref:`Uniforms view<features-uniforms-view>`.
:display-name: The name that is displayed in the :ref:`Uniforms view<features-uniforms-view>` (*string*), if not provided, the **name** property value is displayed with a default formatting (for example, "blurSettings1", would be displayed as "Blur Settings 1").
:parent: Name of a containing group (*string*), if not provided, the group is added to the root.
:index: Group :ref:`order<uniforms-ordering>` priority (*integer*).

Once a uniform group is declared, all the subsequent uniforms in the same file are automatically added to it.
However, uniform groups are only nested if the **parent** property is specified.

.. _uniforms-ordering:

Ordering
--------

Uniforms are always placed before groups, both are ordered according to their index, and order of appearance in the code, where common sources comes first, then Buffers, Image, and Viewers sources. |br|
Negative indexes are relative to the end of the list, where ``-1`` is last, ``-2`` is second before last, and so on.

Storage
-------

If not specified in the :ref:`project definition<definition-project-uniforms>`, uniforms values are stored (on project save) by default alongside the project file, with a :file:`.uniforms.json` suffix. |br|
The uniforms values file is reloaded automatically when modified externally.
