namespace Shaderlens.Render.Resources
{
    using static MarshalExtension;
    using static OpenGL.Gl;

    public interface IVertexArrayResource : IDisposable
    {
        public void Draw();
    }

    public class QuadVertexArrayResource : IVertexArrayResource
    {
        private readonly uint vertexArray;
        private readonly uint buffer;

        private readonly IThreadAccess threadAccess;

        public QuadVertexArrayResource(IThreadAccess threadAccess)
        {
            this.threadAccess = threadAccess;
            this.threadAccess.Verify();

            var quadVertices = new float[]
            {
               // positions
               -1.0f,  1.0f,
               -1.0f, -1.0f,
                1.0f, -1.0f,

               -1.0f,  1.0f,
                1.0f, -1.0f,
                1.0f,  1.0f,
            };

            var quadVerticesPtr = AllocHGlobal(quadVertices);

            this.vertexArray = glGenVertexArray();
            this.buffer = glGenBuffer();

            glBindVertexArray(this.vertexArray);
            glBindBuffer(GL_ARRAY_BUFFER, this.buffer);
            glBufferData(GL_ARRAY_BUFFER, quadVertices.Length * sizeof(float), quadVerticesPtr, GL_STATIC_DRAW);
            glEnableVertexAttribArray(0);
            glVertexAttribPointer(0, 2, GL_FLOAT, false, 2 * sizeof(float), 0);

            Marshal.FreeHGlobal(quadVerticesPtr);
        }

        public void Dispose()
        {
            this.threadAccess.Verify();

            glDeleteVertexArray(this.vertexArray);
            glDeleteBuffer(this.buffer);
        }

        public void Draw()
        {
            glBindVertexArray(this.vertexArray);
            glDrawArrays(GL_TRIANGLES, 0, 6);
            glBindVertexArray(0);
        }
    }
}
