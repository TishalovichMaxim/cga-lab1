using System.Numerics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cga.Drawing;
using Cga.Graphics;
using Cga.LinearAlgebra;
using GlmNet;
using Color = Cga.Drawing.Color;

namespace Cga;

public partial class MainWindow : Window
{
    private WriteableBitmapCanvas _canvas;

    private readonly ObjParser _objParser = new ObjParser();

    private Mesh _mesh;

    private MatrixManager _matrixManager = new();

    private mat4 _model;

    private mat4 _view;

    private mat4 _projection;

    private mat4 _viewport;

    private mat4 _res;

    private vec4 target = new vec4(0, 0, 0, 1);

    private vec4 eye;
    
    public MainWindow()
    {
        InitializeComponent();

        //=======
        
        string path = "Models/Head/";
        
        _mesh = _objParser.Parse(
            path + "head.obj",
            path + "normal.jpg",
            path + "diffuse.jpg",
            path + "specular.png"
            );
        
        eye = new vec4 (0, 2, 4, 1);
        
        //======== 
        
        //string path = "Models/Cube/";
        //
        //_mesh = _objParser.Parse(
        //    path + "cube.obj",
        //    path + "normal.jpg",
        //    path + "diffuse.png",
        //    path + "specular.jpg"
        //    );
        //
        //eye = new vec4 (0, 0, 4, 1);
        
        //=========
        
        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        WriteableBitmap writeableBitmap = new(
            (int)ActualWidth,
            (int)ActualHeight,
            96,
            96,
            PixelFormats.Pbgra32,
            null
            );

        DrawableImage.Source = writeableBitmap;
        
        _canvas = new WriteableBitmapCanvas(writeableBitmap);

        CompositionTarget.Rendering += CompositionTarget_Rendering;

        _model = _matrixManager.GetTranslate(0, 0, 0);
        
        _view = _matrixManager.GetViewMatrix(
            new vec3(0, 1, 0),
            new vec3(eye),
            new vec3(target)
        );
        
        _projection = _matrixManager.GetProjectionMatrix(
            1.0f,
            100.0f,
            ((float)(_canvas.Width))/_canvas.Height,
           50 
        );
        
        _viewport = _matrixManager.GetViewportMatrix(
            _canvas.Width,
            _canvas.Height,
            0,
            0
        );

        _res = _viewport * _projection * _view * _model;
    }

    private void CompositionTarget_Rendering(object sender, EventArgs e)
    {
        Draw();
    }

    public void Draw()
    {
        _canvas.Clear(Color.Red);

        //============
        
        mat4 delta = _matrixManager
            .GetRotateY(1.0f);

        _model = _model*delta;
        
        //============
        
        //_model = _matrixManager.GetRotateX(45.0f);
        
        //============
        
        _res = _viewport * _projection * _view * _model;

        _mesh.Draw(
            _canvas,
            _res,
            new vec3(target - eye),
            _model,
            MeshDrawer.coeffs,
            eye,
            new vec3(Color.Blue.R, Color.Blue.R, Color.Blue.R),
            eye
            );

        _canvas.Swap();
    }
}