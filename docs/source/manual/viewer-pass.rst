.. _viewer-pass:

Viewer Pass
===========

The viewer pass is the last pass of the pipeline, that renders the selected pass framebuffer (Image by default) to the window.

The viewer pass main purpose is to enable viewer transformation (scale and pan), and it also has an opportunity to add extra information to the viewer image.

There are two built in viewers, "None" which only applies the transformation, and "Values Overlay" which also displays the components values inside each pixel (visible when pixels are scaled above a certain level (:ref:`settings<settings-overlay>`)).

A custom viewer pass can be added to the :ref:`Project definition<definition-project>`, and should be responsible for sampling :ref:`iViewerChannel<built-in-uniforms-viewer-channel>` at the transformed position, using the :ref:`iViewerOffset<built-in-uniforms-viewer-offset>` and :ref:`iViewerScale<built-in-uniforms-viewer-scale>` uniforms, before adding extra information.

See the :doc:`Viewer template </appendix/viewer-template>`, and the `Viewers example <https://github.com/ytt0/shaderlens/tree/main/examples/ViewersExample>`_ project for more details.
