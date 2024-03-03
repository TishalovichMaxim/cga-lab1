using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
    
    public MainWindow()
    {
        InitializeComponent();

        _mesh = _objParser.Parse("./Models/cube.obj");
        
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
            new vec3 (0, 0, 4),
            new vec3(0, 0, 0)
        );
        
        _projection = _matrixManager.GetProjectionMatrix(
            0.1f,
            200.0f,
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
        _canvas.Clear();

        _mesh.Draw(_canvas, _res, Color.Blue);
        
        _canvas.Swap();
    }
}