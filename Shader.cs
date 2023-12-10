using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace CSharp_Render
{
    public class Shader
    {
        private int _vertexShader;
        private int _fragmentShader;
        private int _program;

        public Shader(string vertexShaderPath, string fragmentShaderPath)
        {
            string vertexShaderSource = File.ReadAllText(vertexShaderPath);
            string fragmentShaderSource = File.ReadAllText(fragmentShaderPath);

            _vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(_vertexShader, vertexShaderSource);
            GL.CompileShader(_vertexShader);

            int vertexShaderCompileStatus;
            GL.GetShader(_vertexShader, ShaderParameter.CompileStatus, out vertexShaderCompileStatus);
            if (vertexShaderCompileStatus != 1)
            {
                throw new Exception("Vertex shader compilation failed!");
            }

            _fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(_fragmentShader, fragmentShaderSource);
            GL.CompileShader(_fragmentShader);

            int fragmentShaderCompileStatus;
            GL.GetShader(_fragmentShader, ShaderParameter.CompileStatus, out fragmentShaderCompileStatus);
            if (fragmentShaderCompileStatus != 1)
            {
                throw new Exception("Fragment shader compilation failed!");
            }

            _program = GL.CreateProgram();
            GL.AttachShader(_program, _vertexShader);
            GL.AttachShader(_program, _fragmentShader);
            GL.LinkProgram(_program);

            int programLinkStatus;
            GL.GetProgram(_program, (GetProgramParameterName)ProgramParameter.LinkStatus, out programLinkStatus);
            if (programLinkStatus != 1)
            {
                throw new Exception("Shader program linking failed!");
            }
        }

        public void Use()
        {
            GL.UseProgram(_program);
        }

        public int GetAttribLocation(string name)
        {
            return GL.GetAttribLocation(_program, name);
        }

        public int GetUniformLocation(string name)
        {
            return GL.GetUniformLocation(_program, name);
        }

        public void SetUniform(string name, float value)
        {
            int location = GetUniformLocation(name);
            GL.Uniform1(location, value);
        }

        public void SetUniform(string name, Vector3 value)
        {
            int location = GetUniformLocation(name);
            GL.Uniform3(location, value.X, value.Y, value.Z);
        }

        public void SetUniform(string name, Matrix4 value)
        {
            int location = GetUniformLocation(name);
            GL.UniformMatrix4(location, false, ref value);
        }
    }
}