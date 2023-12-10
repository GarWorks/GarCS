using CSharp_Render;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

public class Mesh
{
    private List<Vector3> _vertices;
    private List<Vector3> _normals;
    private List<Vector2> _textureCoordinates;
    private List<uint> _indices;

    public Vector3[] Vertices => _vertices.ToArray();
    public Vector3[] Normals => _normals.ToArray();
    public Vector2[] TextureCoordinates => _textureCoordinates.ToArray();
    public uint[] Indices => _indices.ToArray();

    private int _vao;
    private int _vbo;
    private int _ebo;

    public Mesh(string filePath)
    {
        _vertices = new List<Vector3>();
        _normals = new List<Vector3>();
        _textureCoordinates = new List<Vector2>();
        _indices = new List<uint>();

        LoadOBJ(filePath);
        CreateBuffers();
    }

    private void LoadOBJ(string filePath)
    {
        using (var reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var parts = line.Split(' ');

                if (parts[0] == "v")
                {
                    _vertices.Add(new Vector3(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3])));
                }
                else if (parts[0] == "vn")
                {
                    _normals.Add(new Vector3(float.Parse(parts[1]), float.Parse(parts[2]), float.Parse(parts[3])));
                }
                else if (parts[0] == "vt")
                {
                    _textureCoordinates.Add(new Vector2(float.Parse(parts[1]), float.Parse(parts[2])));
                }
                else if (parts[0] == "f")
                {
                    for (int i = 1; i < parts.Length - 2; i++)
                    {
                        var vertexData = parts[i].Split('/');
                        var index = uint.Parse(vertexData[0]) - 1;
                        _indices.Add(index);

                        if (vertexData.Length > 2)
                        {
                            index = uint.Parse(vertexData[2]) - 1;
                            _indices.Add(index);
                        }
                    }
                }
            }
        }
    }

    private void CreateBuffers()
    {
        _vao = GL.GenVertexArray();
        _vbo = GL.GenBuffer();
        _ebo = GL.GenBuffer();

        GL.BindVertexArray(_vao);

        GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        GL.BufferData(BufferTarget.ArrayBuffer, Vertices.Length * Vector3.SizeInBytes, Vertices, BufferUsageHint.StaticDraw);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);
        GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * sizeof(uint), Indices, BufferUsageHint.StaticDraw);

        GL.BindVertexArray(0);
    }

    public void Render(Shader shader)
    {
        GL.BindVertexArray(_vao);
        GL.BindBuffer(BufferTarget.ElementArrayBuffer, _ebo);

        int vertexLocation = shader.GetAttribLocation("a_Position");
        int normalLocation = shader.GetAttribLocation("a_Normal");
        int textureCoordLocation = shader.GetAttribLocation("a_TexCoord");

        GL.EnableVertexAttribArray(vertexLocation);
        GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 0, 0);

        if (_normals.Count > 0)
        {
            GL.EnableVertexAttribArray(normalLocation);
            GL.VertexAttribPointer(normalLocation, 3, VertexAttribPointerType.Float, false, 0, 0);
        }

        if (_textureCoordinates.Count > 0)
        {
            GL.EnableVertexAttribArray(textureCoordLocation);
            GL.VertexAttribPointer(textureCoordLocation, 2, VertexAttribPointerType.Float, false, 0, 0);
        }

        GL.DrawElements(PrimitiveType.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);

        GL.DisableVertexAttribArray(vertexLocation);
        if (_normals.Count > 0) GL.DisableVertexAttribArray(normalLocation);
        if (_textureCoordinates.Count > 0) GL.DisableVertexAttribArray(textureCoordLocation);

        GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        GL.BindVertexArray(0);
    }
}
