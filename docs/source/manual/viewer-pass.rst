.. _viewer-pass:

Viewer Pass
===========

The viewer pass is the last pass of the pipeline, that renders the selected buffer (Image by default) to the window.

The viewer pass main purpose is to enable viewer transformation (scale and pan), and it also has an opportunity to add extra information to the viewer image.

There are two built in viewers, "None" which only applies the transformation, and "Values Overlay" which also adds components value information inside each pixel (visible when pixels are scaled above a certain level (:ref:`settings<settings-overlay>`)).

A custom viewer pass can be added to the :ref:`Project definition<definition-project>`, and should be responsible for applying the viewer transformation (using :ref:`iViewerOffset<built-in-uniforms-viewer-offset>`, and :ref:`iViewerScale<built-in-uniforms-viewer-scale>` uniforms), before adding extra information.

See the :doc:`/appendix/viewer-template`, and the `ViewersExample <https://ytt0x.github.com/Shaderlens/examples/ViewersExample>`_ project for implementation examples.
