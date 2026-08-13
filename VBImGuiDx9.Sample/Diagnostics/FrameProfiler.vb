Option Strict On
Option Explicit On
Option Infer On

Imports System
Imports System.Diagnostics

Namespace VBImGuiDx9.Sample.Diagnostics

    Public NotInheritable Class FrameProfiler

        Private Const SampleCount As Integer = 120

        Private ReadOnly _stopwatch As Stopwatch =
            Stopwatch.StartNew()

        Private ReadOnly _frameSamples(
            SampleCount - 1) As Double

        Private _sampleIndex As Integer
        Private _sampleCountActual As Integer

        Private _inputMs As Double
        Private _deviceMs As Double
        Private _imguiBuildMs As Double
        Private _imguiRenderMs As Double
        Private _dx9DrawMs As Double
        Private _presentMs As Double
        Private _frameMs As Double

        Private _frames As Long

        Public ReadOnly Property OtherMs As Double
            Get

                Dim value As Double =
            _frameMs -
            _inputMs -
            _deviceMs -
            _imguiBuildMs -
            _imguiRenderMs -
            _dx9DrawMs -
            _presentMs

                If value < 0.0 Then
                    Return 0.0
                End If

                Return value

            End Get
        End Property

        Public ReadOnly Property Frames As Long
            Get
                Return _frames
            End Get
        End Property

        Public ReadOnly Property InputMs As Double
            Get
                Return _inputMs
            End Get
        End Property

        Public ReadOnly Property DeviceMs As Double
            Get
                Return _deviceMs
            End Get
        End Property

        Public ReadOnly Property ImGuiBuildMs As Double
            Get
                Return _imguiBuildMs
            End Get
        End Property

        Public ReadOnly Property ImGuiRenderMs As Double
            Get
                Return _imguiRenderMs
            End Get
        End Property

        Public ReadOnly Property Dx9DrawMs As Double
            Get
                Return _dx9DrawMs
            End Get
        End Property

        Public ReadOnly Property PresentMs As Double
            Get
                Return _presentMs
            End Get
        End Property

        Public ReadOnly Property FrameMs As Double
            Get
                Return _frameMs
            End Get
        End Property

        Public ReadOnly Property MinFrameMs As Double
            Get
                Return GetPercentile(
                    0.0)
            End Get
        End Property

        Public ReadOnly Property P50FrameMs As Double
            Get
                Return GetPercentile(
                    0.5)
            End Get
        End Property

        Public ReadOnly Property P95FrameMs As Double
            Get
                Return GetPercentile(
                    0.95)
            End Get
        End Property

        Public ReadOnly Property P99FrameMs As Double
            Get
                Return GetPercentile(
                    0.99)
            End Get
        End Property

        Public ReadOnly Property MaxFrameMs As Double
            Get
                Return GetPercentile(
                    1.0)
            End Get
        End Property

        Public ReadOnly Property AverageFrameMs As Double
            Get

                If _sampleCountActual = 0 Then
                    Return 0.0
                End If

                Dim total As Double = 0.0

                For i As Integer = 0 To _sampleCountActual - 1
                    total += _frameSamples(i)
                Next

                Return total /
                       _sampleCountActual

            End Get
        End Property

        Public ReadOnly Property AverageFps As Double
            Get

                Dim average As Double =
                    AverageFrameMs

                If average <= 0.0 Then
                    Return 0.0
                End If

                Return 1000.0 /
                       average

            End Get
        End Property

        Public ReadOnly Property SampleCountCurrent As Integer
            Get
                Return _sampleCountActual
            End Get
        End Property

        Public Sub BeginInput()
            _stopwatch.Restart()
        End Sub

        Public Sub EndInput()
            _inputMs =
                _stopwatch.Elapsed.TotalMilliseconds
        End Sub

        Public Sub BeginDevice()
            _stopwatch.Restart()
        End Sub

        Public Sub EndDevice()
            _deviceMs =
                _stopwatch.Elapsed.TotalMilliseconds
        End Sub

        Public Sub BeginImGuiBuild()
            _stopwatch.Restart()
        End Sub

        Public Sub EndImGuiBuild()
            _imguiBuildMs =
                _stopwatch.Elapsed.TotalMilliseconds
        End Sub

        Public Sub BeginImGuiRender()
            _stopwatch.Restart()
        End Sub

        Public Sub EndImGuiRender()
            _imguiRenderMs =
                _stopwatch.Elapsed.TotalMilliseconds
        End Sub

        Public Sub BeginDx9Draw()
            _stopwatch.Restart()
        End Sub

        Public Sub EndDx9Draw()
            _dx9DrawMs =
                _stopwatch.Elapsed.TotalMilliseconds
        End Sub

        Public Sub BeginPresent()
            _stopwatch.Restart()
        End Sub

        Public Sub EndPresent()
            _presentMs =
                _stopwatch.Elapsed.TotalMilliseconds
        End Sub

        Public Sub RecordFrame(
            frameMs As Double)

            _frameMs = frameMs

            _frameSamples(_sampleIndex) =
                frameMs

            _sampleIndex += 1

            If _sampleIndex >= SampleCount Then
                _sampleIndex = 0
            End If

            If _sampleCountActual < SampleCount Then
                _sampleCountActual += 1
            End If

            _frames += 1

        End Sub

        Public Sub Reset()

            Array.Clear(
                _frameSamples,
                0,
                _frameSamples.Length)

            _sampleIndex = 0
            _sampleCountActual = 0

            _inputMs = 0.0
            _deviceMs = 0.0
            _imguiBuildMs = 0.0
            _imguiRenderMs = 0.0
            _dx9DrawMs = 0.0
            _presentMs = 0.0
            _frameMs = 0.0

            _frames = 0

        End Sub

        Private Function GetPercentile(
            percentile As Double) As Double

            If _sampleCountActual = 0 Then
                Return 0.0
            End If

            Dim values(
                _sampleCountActual - 1) As Double

            Array.Copy(
                _frameSamples,
                values,
                _sampleCountActual)

            Array.Sort(values)

            If percentile <= 0.0 Then
                Return values(0)
            End If

            If percentile >= 1.0 Then
                Return values(values.Length - 1)
            End If

            Dim position As Double =
                percentile *
                (values.Length - 1)

            Dim lower As Integer =
                CInt(Math.Floor(position))

            Dim upper As Integer =
                CInt(Math.Ceiling(position))

            If lower = upper Then
                Return values(lower)
            End If

            Dim fraction As Double =
                position - lower

            Return values(lower) +
                   (values(upper) -
                    values(lower)) *
                   fraction

        End Function

    End Class

End Namespace