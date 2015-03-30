using System;
using System.Collections.Generic;
using System.Linq;
using amulware.Graphics;
using amulware.Graphics.ShaderManagement;
using Bearded.Utilities;
using Bearded.Utilities.Input;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace GenericGamedev.Pixelation
{
    sealed class GameWindow : amulware.Graphics.Program
    {
        private ShaderManager shaderMan;
        private int activeShaderIndex;
        private List<PostProcessSurface> surfaces;
        private Vector2Uniform mousePosition;
        private int glWidth;
        private int glHeight;
        private Vector2Uniform screenSize;
        private List<ShaderFile> fragmentShaders;

        public GameWindow()
            :base(1280, 720, GraphicsMode.Default,
            "GameDev<T> Pixelation Post Processing",
            GameWindowFlags.Default, DisplayDevice.Default,
            3, 0, GraphicsContextFlags.ForwardCompatible)
        {
        }

        protected override void OnLoad(EventArgs e)
        {
            this.shaderMan = new ShaderManager();

            var shaderLoader = ShaderFileLoader.CreateDefault("shaders");

            // load all shaders
            var shaders = shaderLoader.Load("").ToList();
            this.shaderMan.Add(shaders);

            this.fragmentShaders = shaders
                .Where(s => s.Type == ShaderType.FragmentShader)
                .ToList();

            // create programs for all fragment shaders
            var shaderPrograms = this.fragmentShaders
                .Select(fs => this.shaderMan
                    .MakeShaderProgram()
                    .WithVertexShader("post")
                    .WithFragmentShader(fs.FriendlyName)
                    .As(fs.FriendlyName)
                ).ToList();

            var texture = new Texture("texture.jpg");
            texture.SetParameters(TextureMinFilter.Linear, TextureMagFilter.Linear, TextureWrapMode.Clamp, TextureWrapMode.Clamp);

            var surfaceSettings = new SurfaceSetting[]
            {
                new TextureUniform("diffuseTexture", texture),
                this.mousePosition = new Vector2Uniform("mousePosition"),
                this.screenSize = new Vector2Uniform("screenSize"),
            };

            // create post processing surfaces for all programs
            this.surfaces = shaderPrograms
                .Select(p =>
                {
                    var surface = new PostProcessSurface();
                    p.UseOnSurface(surface);
                    surface.AddSettings(surfaceSettings);
                    return surface;
                }
                ).ToList();

            this.activeShaderIndex = 0;

            InputManager.Initialize(this.Mouse);

            Log.Line("Loaded shaders:");

            foreach (var shader in this.fragmentShaders)
            {
                Log.Line(shader.FriendlyName);
            }
            Log.Line("-------------------------------------------------");
            Log.Line("Switch shaders with space bar");
            Log.Line("Control shader parameters by moving mouse");
            Log.Line("-------------------------------------------------");
            Log.Line("starting with shader '{0}'", this.fragmentShaders[this.activeShaderIndex].FriendlyName);
        }

        protected override void OnUpdate(UpdateEventArgs e)
        {
            if (this.Focused)
            {
                InputManager.Update();

                this.mousePosition.Vector = InputManager.MousePosition;

                // switch between shaders with space bar
                if (InputManager.IsKeyHit(Key.Space))
                {
                    this.activeShaderIndex = (this.activeShaderIndex + 1) % this.surfaces.Count;
                    Log.Line("switched to shader '{0}'", this.fragmentShaders[this.activeShaderIndex].FriendlyName);
                }
            }
        }

        protected override void OnRender(UpdateEventArgs e)
        {
            // resize viewport if needed
            if (this.Height != this.glHeight || this.Width != this.glWidth)
            {
                this.glHeight = this.Height;
                this.glWidth = this.Width;
                GL.Viewport(0, 0, this.glWidth, this.glHeight);
                this.screenSize.Vector = new Vector2(this.glWidth, this.glHeight);
            }

            GL.ClearColor(Color.Purple);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            // render active post processing surface
            this.surfaces[this.activeShaderIndex].Render();

            this.SwapBuffers();
        }
    }
}
