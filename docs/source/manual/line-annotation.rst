Line Annotations
================

A line annotation is a single-line comment that can be added to source files (GLSL or C#), and get processed by the viewer when loaded.

Line annotations are used for defining :doc:`uniforms` properties and groups, and for C# to GLSL :doc:`conversion rules</manual/csharp/conversion-rules>`.

Each line annotation starts with :csharp:`//@`, followed by a type, an optional value, and an optional list of properties.

Type only:
    :csharp:`//@type`

    :type: An identifier, containing word characters and dash, for example ``uniform-group``.

Type, value, and a list of properties:
    :csharp:`//@type, key1: value1, key2: value2` |br|
    :csharp:`//@type: value, key1: value1, key2: value2`

    :key: An identifier.
    :value:

        - A number.
        - A string, surrounded with quotation marks (``"..."``), and ``\`` as an escape character (used with ``\"``, ``\\``, ``\r``, ``\n``).
        - An identifier, or an expression that starts with an identifier, for example ``vec2(1.0, 2.0)``.

Type and a single value:
    :csharp:`//@type: line-end-terminated-value`

    :value: A string, without quotation marks or escape character, that ends at the end of the line.

Examples:
    - :csharp:`//@define`
    - :csharp:`//@uniform, step: vec3(0.1, 0.2, 0.3)`
    - :csharp:`//@uniform-group, display-name: "Blur Settings", index: 5`
    - :csharp:`//@replace-line: #define screen(color1, color2) (1.0 - (1.0 - (color1)) * (1.0 - (color2)))`
