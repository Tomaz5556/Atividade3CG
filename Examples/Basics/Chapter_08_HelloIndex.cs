using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace _3dCG.Examples.Basics
{
    internal class Chapter_08_HelloIndex : GameWindow
    {

        private const int POSITION = 0;
        private const int COLOR = 1;
        private const int UV = 2;
        private readonly int[] OFFSET = { 0, 12, 24 };
        private const int VERTEX_SIZE = 8 * sizeof(float);

        private int _indexCount = 0;

        private int _vertexBufferObject;
        private int _vertexArrayObject;
        private int _indexBuffer;

        private Shader _shader;
        private Texture _texture;

        // Variáveis que armazenam a localização dos uniforms "time" e "mousePosition" no shader
        private int _timeUniformLocation;
        private int _mousePositionUniformLocation;

        // Variáveis que armazenam o tempo decorrido e a posição do mouse
        private float _time = 0.0f;
        private Vector2 _mousePosition = new Vector2(0.0f, 0.0f);

        public Chapter_08_HelloIndex(
            GameWindowSettings gameWindowSettings,
            NativeWindowSettings nativeWindowSettings) :
            base(gameWindowSettings, nativeWindowSettings)
        {
            Title = "Hello Index!";
        }

        protected override void OnLoad()
        {
            base.OnLoad();

            float[] _data =
            {
                // Position      // Color          // Uv coords
                -1.0f, -1.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 0.0f, // Bottom-left
                 1.0f, -1.0f, 0.0f, 0.0f, 1.0f, 0.0f, 1.0f, 0.0f, // Bottom-right
                 1.0f,  1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 1.0f, 1.0f, // Top-right
                -1.0f,  1.0f, 0.0f, 1.0f, 1.0f, 1.0f, 0.0f, 1.0f  // Top-left
            };


            int[] _indices =
            {
                0, 1, 2, // First triangle
                0, 2, 3  // Second triangle
            };

            _indexCount = _indices.Length;



            // Generate the buffer
            _vertexBufferObject = GL.GenBuffer();
            // Points to the active buffer
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject);
            // Insert the data into the buffer
            GL.BufferData(BufferTarget.ArrayBuffer, _data.Length * sizeof(float), _data, BufferUsageHint.StaticDraw);
            // Generate the array object buffer
            _vertexArrayObject = GL.GenVertexArray();
            // Points to the array object
            GL.BindVertexArray(_vertexArrayObject);
            // Position attribute
            GL.VertexAttribPointer(POSITION, 3, VertexAttribPointerType.Float, false, VERTEX_SIZE, OFFSET[POSITION]);
            GL.EnableVertexAttribArray(POSITION);
            // Color attribute
            GL.VertexAttribPointer(COLOR, 3, VertexAttribPointerType.Float, false, VERTEX_SIZE, OFFSET[COLOR]);
            GL.EnableVertexAttribArray(COLOR);
            // Texture coordinates attribute
            GL.VertexAttribPointer(UV, 2, VertexAttribPointerType.Float, false, VERTEX_SIZE, OFFSET[UV]);
            GL.EnableVertexAttribArray(UV);

            // Create and bind index buffer (information about the faces triangulation)
            _indexBuffer = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBuffer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, sizeof(int) * _indexCount, _indices, BufferUsageHint.StaticDraw);

            _shader = new Shader("HelloUniform");

            // Textura personalizada
            _texture = Texture.LoadFromFile("Resources/Texture/xadrez.png", TextureUnit.Texture0);

            // Pegando a localização dos uniforms "time" e "mousePosition" no shader
            _timeUniformLocation = GL.GetUniformLocation(_shader.Handle, "time");
            _mousePositionUniformLocation = GL.GetUniformLocation(_shader.Handle, "mousePosition");

            GL.ClearColor(0.1f, 0.1f, 0.2f, 1.0f);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            GL.Clear(ClearBufferMask.ColorBufferBit);

            _texture.Use(TextureUnit.Texture0);
            _shader.Use();

            // Atualização dos valores dos uniforms "time" e "mousePosition" no shader
            GL.Uniform1(_timeUniformLocation, _time);
            GL.Uniform2(_mousePositionUniformLocation, _mousePosition);

            GL.BindVertexArray(_vertexArrayObject);
            GL.DrawElements(BeginMode.Triangles, _indexCount, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);

            SwapBuffers();
        }

        // Função chamada a cada quadro. Aqui, o tempo é incrementado a cada quadro
        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);

            _time += 0.01f;
        }

        // Função chamada sempre que o mouse se move. Aqui, a posição do mouse é atualizada
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            base.OnMouseMove(e);

            _mousePosition = new Vector2(e.X / Size.X, e.Y / Size.Y);
        }

        protected override void OnUnload()
        {
            base.OnUnload();

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.DeleteBuffer(_vertexBufferObject);
            GL.DeleteVertexArray(_vertexArrayObject);
        }
    }
}