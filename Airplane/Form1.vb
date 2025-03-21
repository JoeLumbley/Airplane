Imports System.Drawing.Drawing2D
Imports System.IO
Imports System.Math
Imports System.Runtime.InteropServices
Imports System.Text

Public Structure DeltaTimeStructure

    Public CurrentFrame As DateTime
    Public LastFrame As DateTime
    Public ElapsedTime As TimeSpan

    Public Sub New(currentFrame As Date, lastFrame As Date,
                       elapsedTime As TimeSpan)

        Me.CurrentFrame = currentFrame
        Me.LastFrame = lastFrame
        Me.ElapsedTime = elapsedTime
    End Sub

    Public Sub Update()

        ' Set the current frame's time to the current system time.
        CurrentFrame = Now

        ' Calculates the elapsed time ( delta time Δt ) between the
        ' current frame and the last frame.
        ElapsedTime = CurrentFrame - LastFrame

        ' Updates the last frame's time to the current frame's time for
        ' use in the next update.
        LastFrame = CurrentFrame

    End Sub

    Public Sub LastFrameNow()

        ' Set the current frame's time to the current system time.
        LastFrame = Now

    End Sub

End Structure




Public Structure Turret

    Public Pen As Pen
    Public UnderlightPen As Pen
    Public UnderlightBrush As Brush
    Public Center As PointF
    Public Length As Integer
    Public AngleInDegrees As Single
    Public TimeToNextFire As TimeSpan
    Public LastFireTime As DateTime
    Public Diameter As Integer



    Public Sub New(pen As Pen, diameter As Integer, center As PointF, length As Integer, angleInDegrees As Single, reloadTime As TimeSpan)

        Me.Pen = pen
        Me.Center = center
        Me.Length = length
        Me.AngleInDegrees = angleInDegrees
        Me.TimeToNextFire = reloadTime
        UnderlightPen = New Pen(Color.FromArgb(128, Color.Blue), 14)
        UnderlightBrush = New SolidBrush(Color.FromArgb(128, Color.Blue))
        Me.Diameter = diameter



    End Sub

    Public Sub Draw(g As Graphics)
        ' Draw a line of given length from the given center point at a given angle.


        Dim UnderLightRectangle As New Rectangle(Center.X - Diameter / 2, Center.Y - Diameter / 2, Diameter, Diameter)

        UnderLightRectangle.Inflate(2, 2)

        g.FillEllipse(UnderlightBrush, UnderLightRectangle)

        DrawEllipseWithRadialGradient(g, Center, Diameter)

        ' Draw Barrel
        DrawLineFromCenterGivenLenghtAndAngle(g, UnderlightPen, Center, Length, AngleInDegrees)

        DrawLineFromCenterGivenLenghtAndAngle(g, Pen, Center, Length, AngleInDegrees)


    End Sub

    Public Sub DrawEllipseWithRadialGradient(g As Graphics, center As PointF, diameter As Integer)

        ' Create the path for the ellipse
        Dim path As New GraphicsPath()

        Dim GradRect As New Rectangle(center.X - diameter / 2, center.Y - diameter / 2, diameter, diameter)

        GradRect.Inflate(4, 4)

        path.AddEllipse(GradRect)

        ' Create the radial gradient brush
        Dim brush As New PathGradientBrush(path)

        ' Set the center color (highlight)
        brush.CenterColor = Color.White

        ' Set the surrounding colors (background)
        brush.SurroundColors = New Color() {Color.Black}

        ' Set the center point of the gradient to the top left quadrant
        brush.CenterPoint = New PointF(center.X - diameter / 4, center.Y - diameter / 4)

        ' Fill the ellipse with the radial gradient brush
        g.FillEllipse(brush, New Rectangle(center.X - diameter / 2, center.Y - diameter / 2, diameter, diameter))

    End Sub

    Private Sub DrawLineFromCenterGivenLenghtAndAngle(g As Graphics, pen As Pen, center As PointF, length As Integer, angleInDegrees As Single)
        ' Draw a line of given length from the given center point at a given angle.

        Dim angleInRadians As Single = angleInDegrees * (Math.PI / 180) ' Convert to radians

        Dim EndPoint As PointF

        ' Calculate the endpoint of the line using trigonometry
        EndPoint = New PointF(center.X + length * Cos(angleInRadians),
                              center.Y + length * Sin(angleInRadians))

        ' Draw the line.
        g.DrawLine(pen, center, EndPoint)

    End Sub

End Structure

Public Structure ProjectileManager

    Public Brush As Brush
    Public BarrelLength As Integer
    Public MuzzleVelocity As Single
    Public Size As Size
    Public LifeTimeInSeconds As Integer


    Public Sub New(brush As Brush, size As Size, muzzleVelocity As Single, barrelLength As Integer, lifeTimeInSeconds As Integer)

        Me.Brush = brush
        Me.BarrelLength = barrelLength
        Me.MuzzleVelocity = muzzleVelocity
        Me.Size = size
        Me.LifeTimeInSeconds = lifeTimeInSeconds

    End Sub

    Private Structure Projectile

        ' Array of projectiles
        '   Fire
        '       Add
        '   Update
        '       Move
        '       Remove
        '   Draw
        '   Collide

        Public X, Y, Width, Height As Double

        Public Velocity As PointF

        Public Brush As Brush

        Public Center As PointF

        Public BarrelLength As Integer

        Public AngleInDegrees As Single

        Public Creation As DateTime

        Public Sub New(brush As Brush, width As Double, height As Double, velocity As Single, center As PointF, length As Integer, angleInDegrees As Single)

            Me.Brush = brush

            Me.Width = width
            Me.Height = height

            Me.Center = center
            Me.BarrelLength = length

            Me.AngleInDegrees = angleInDegrees

            ' Convert angle from degrees to radians.
            Dim AngleInRadians As Single = angleInDegrees * (Math.PI / 180)

            ' Set initial position based on the angle and length
            X = center.X + length * Cos(AngleInRadians) - Me.Width / 2
            Y = center.Y + length * Sin(AngleInRadians) - Me.Height / 2

            ' Set velocity based on angle
            Me.Velocity = New PointF(Cos(AngleInRadians) * velocity,
                                     Sin(AngleInRadians) * velocity)

            ' To have the max impact think tutuorial for a young bill gates.

            Creation = Now()

        End Sub

        Public Sub UpdateMovement(ByVal deltaTime As TimeSpan)

            'Move our projectile horizontally.
            X += Velocity.X * deltaTime.TotalSeconds 'Δs = V * Δt
            'Displacement = Velocity x Delta Time

            'Move our projectile vertically.
            Y += Velocity.Y * deltaTime.TotalSeconds 'Δs = V * Δt
            'Displacement = Velocity x Delta Time

        End Sub

        ' Methods to round attributes to
        ' the nearest integer values.
        Public Function NearestX() As Integer

            Return RoundToNearest(X)
        End Function
        Public Function NearestY() As Integer

            Return RoundToNearest(Y)
        End Function
        Public Function NearestWidth() As Integer

            Return RoundToNearest(Width)
        End Function
        Public Function NearestHeight() As Integer

            Return RoundToNearest(Height)
        End Function

        ' Generic method to round attributes to the nearest integer values.
        Private Function RoundToNearest(ByVal value As Double) As Integer

            Return CInt(Math.Round(value))
        End Function

        Public Function Rectangle() As Rectangle

            Return New Rectangle(NearestX, NearestY, NearestWidth, NearestHeight)
        End Function

    End Structure

    Private Projectiles() As Projectile

    Public Sub FireProjectile(CenterOfFire As PointF, AngleInDegrees As Single)

        AddProjectile(CenterOfFire, AngleInDegrees)

    End Sub

    Public Sub UpdateProjectiles(deltaTime As TimeSpan)

        If Projectiles IsNot Nothing Then

            RemoveProjectilesPastTheirLifeTime()

            For Index As Integer = 0 To Projectiles.Length - 1

                Projectiles(Index).UpdateMovement(deltaTime)

            Next

        End If

    End Sub

    Public Sub DrawProjectiles(graphics As Graphics)

        If Projectiles IsNot Nothing Then

            For Each projectile In Projectiles

                graphics.FillRectangle(projectile.Brush, projectile.Rectangle())

            Next

        End If

    End Sub

    Private Sub AddProjectile(centerOfFire As PointF, angleInDegrees As Single)

        If Projectiles Is Nothing Then

            ReDim Projectiles(0)

        Else

            Array.Resize(Projectiles, Projectiles.Length + 1)

        End If

        Projectiles(Projectiles.Length - 1) = New Projectile(Brush,
                                                             Size.Width,
                                                             Size.Height,
                                                             MuzzleVelocity,
                                                             centerOfFire,
                                                             BarrelLength,
                                                             angleInDegrees)

    End Sub

    Public Function IsColliding(rectangle As Rectangle) As Boolean

        If Projectiles IsNot Nothing Then

            Return Projectiles.Any(Function(p) p.Rectangle().IntersectsWith(rectangle))

        End If

        ' Projectiles.Any(Function(p) p.Rectangle().IntersectsWith(rectangle))

        ' This is a LINQ lambda expression.

        ' The Any method with a lambda expression is a concise and efficient
        ' way to check if any projectile intersects with the given rectangle.

        ' Function(p) p.Rectangle().IntersectsWith(rectangle)

        Return False

    End Function

    Public Sub RemoveCollidingProjectiles(rectangle As Rectangle)
        ' Remove projectiles that collide with the given rectangle.

        ' Check for projectiles that collide.
        Projectiles = Projectiles.Where(Function(p) Not p.Rectangle().IntersectsWith(rectangle)).ToArray()

        ' Projectiles.Where(Function(p) Not p.Rectangle().IntersectsWith(rectangle))

        ' This is a LINQ lambda expression.

        ' The Where method filters the Projectiles array based on the condition
        ' provided by the lambda function.

        ' Not p.Rectangle().IntersectsWith(rectangle)

    End Sub

    Private Sub RemoveProjectilesPastTheirLifeTime()
        ' To prevent a slow down and to reduce memory use.

        Dim lifeTime As Integer = LifeTimeInSeconds

        ' Filter projectiles past their life time.
        Projectiles = Projectiles.Where(Function(p) (Date.Now - p.Creation).TotalSeconds < lifeTime).ToArray()

        ' Projectiles.Where(Function(p) (Date.Now - p.Creation).TotalSeconds < lifeTime)

        ' This is a LINQ lambda expression.

        ' The Where method filters the Projectiles array based on the condition
        ' provided by the lambda function.

        ' (Date.Now - p.Creation).TotalSeconds < lifeTime)

        ' ElapsedTimeFromCreation.TotalSeconds < lifeTime 
        '                            0.0003402 < 9        Alive
        '                          133.0161469 < 9        Past Life Time

        ' Using LINQ with lambda expressions is a powerful way to perform queries
        ' and manipulate collections in a concise and readable manner.

        ' LINQ - https://learn.microsoft.com/en-us/dotnet/visual-basic/programming-guide/concepts/linq/

        ' Lambda Expressions - https://learn.microsoft.com/en-us/dotnet/visual-basic/programming-guide/language-features/procedures/lambda-expressions

    End Sub

End Structure






Public Structure ArrowVector

    Public Pen As Pen

    Public OutlinePen As Pen


    Public ReversePen As Pen

    Public Center As PointF

    Public Velocity As Double

    Public VelocityVector As PointF

    Public Acceleration As PointF

    Public MinVelocity As Integer

    Public MaxVelocity As Double

    Public Length As Integer

    Public ReverseLength As Integer

    Public MinLength As Integer

    Public MaxLength As Integer

    Public Width As Single

    Public ReverseWidth As Single

    Public MinWidth As Integer

    Public MaxWidth As Integer

    Public AngleInDegrees As Single

    Public AngleInRadians As Single

    Public EndPoint As PointF

    Public OutlineEndPoint As PointF


    Public ReverseEndPoint As PointF

    Public Sub New(pen As Pen,
                   center As PointF,
                   angleInDegrees As Single,
                   minLength As Double,
                   maxLength As Double,
                   minWidth As Double,
                   maxWidth As Double,
                   velocity As Double,
                   maxVelocity As Double, acceleration As Double)

        Me.Pen = pen

        pen.StartCap = Drawing2D.LineCap.Round

        pen.EndCap = Drawing2D.LineCap.ArrowAnchor


        OutlinePen = New Pen(Color.White, Me.Pen.Width + 4)

        OutlinePen.StartCap = Drawing2D.LineCap.Round

        OutlinePen.EndCap = Drawing2D.LineCap.ArrowAnchor



        ReversePen = New Pen(Color.White, 5)

        ReversePen.StartCap = Drawing2D.LineCap.Round

        ReversePen.EndCap = Drawing2D.LineCap.ArrowAnchor

        Me.Center = center

        If angleInDegrees >= 0 AndAlso angleInDegrees <= 360 Then
            Me.AngleInDegrees = angleInDegrees
        Else
            Me.AngleInDegrees = 0
        End If

        AngleInRadians = DegreesToRadians(angleInDegrees)

        ' Calculate the endpoint of the line using trigonometry
        EndPoint = New PointF(center.X + Length * Cos(AngleInRadians),
                              center.Y + Length * Sin(AngleInRadians))

        Me.MinLength = minLength
        Me.MaxLength = maxLength

        Me.MinWidth = minWidth
        Me.MaxWidth = maxWidth

        Me.Velocity = velocity
        Me.MaxVelocity = maxVelocity

        ' Set velocity based on angle
        VelocityVector.X = Cos(AngleInRadians) * Me.Velocity
        VelocityVector.Y = Sin(AngleInRadians) * Me.Velocity

        Length = GetLength(Me.Velocity, Me.MaxVelocity, Me.MinLength, Me.MaxLength)

        Width = GetWidth(Me.Velocity, Me.MaxVelocity, Me.MinWidth, Me.MaxWidth)

        pen.Width = Width

        ReverseLength = GetReverseLength(velocity, maxVelocity, minLength, maxLength)

        ReverseWidth = GetReverseWidth(velocity, maxVelocity, minWidth, maxWidth)

        ReversePen.Width = ReverseWidth

        Me.Acceleration.X = acceleration
        Me.Acceleration.Y = acceleration

    End Sub

    Public Sub Update(ByVal deltaTime As TimeSpan)

        Length = GetLength(Velocity, MaxVelocity, MinLength, MaxLength)

        Width = GetWidth(Velocity, MaxVelocity, MinWidth, MaxWidth)

        Pen.Width = Width

        OutlinePen.Width = Width + 4



        ReversePen.Width = ReverseWidth

        AngleInRadians = DegreesToRadians(AngleInDegrees)

        ' Set velocity based on angle
        VelocityVector.X = Cos(AngleInRadians) * Velocity
        VelocityVector.Y = Sin(AngleInRadians) * Velocity

        ' Calculate the endpoint of the line using trigonometry
        EndPoint = New PointF(Center.X + Length * Cos(AngleInRadians),
                              Center.Y + Length * Sin(AngleInRadians))


        ' Calculate the endpoint of the line using trigonometry
        OutlineEndPoint = New PointF(Center.X + (Length + 4) * Cos(AngleInRadians),
                                     Center.Y + (Length + 4) * Sin(AngleInRadians))



        ReverseLength = GetReverseLength(Velocity, MaxVelocity, MinLength, MaxLength)

        ReverseWidth = GetReverseWidth(Velocity, MaxVelocity, MinWidth, MaxWidth)

        ' Calculate the reverse endpoint.
        ReverseEndPoint = New PointF(Center.X + ReverseLength * Math.Cos(ReverseAngleInRadians(AngleInDegrees)),
                             Center.Y + ReverseLength * Math.Sin(ReverseAngleInRadians(AngleInDegrees)))

        UpdateMovement(deltaTime)

    End Sub

    Public Sub UpdateMovement(ByVal deltaTime As TimeSpan)

        'Move our arrow horizontally.
        Center.X += CSng(VelocityVector.X * deltaTime.TotalSeconds)

        'Move our arrow vertically.
        Center.Y += CSng(VelocityVector.Y * deltaTime.TotalSeconds)

        'Δs = V * Δt
        'Displacement = Velocity x Delta Time

    End Sub

    Public Sub Draw(g As Graphics)

        ' Draw reverse arrow (Negative Vector ←)
        'g.DrawLine(ReversePen, Center, ReverseEndPoint)
        ' We are visualizing our negative vector as an arrow that points in the
        ' opposite direction.



        ' Draw forward arrow outline (Vector →)
        g.DrawLine(OutlinePen, Center, OutlineEndPoint)

        ' Draw forward arrow (Vector →)
        g.DrawLine(Pen, Center, EndPoint)
        ' We are visualizing our vector as an arrow.



        ' Vector →
        ' A vector is a mathematical object that has both a magnitude (or length)
        ' and a direction.

        ' Negative Vector ←
        ' The opposite or negative of a vector is another vector that has the same
        ' magnitude but points in the opposite direction.

    End Sub

    Function GetLength(velocity As Double, maxVelocity As Double, minlength As Double, maxlength As Double) As Double

        ' Normalize the velocity
        Dim NormalizedVelocity As Double = velocity / maxVelocity

        ' Interpolate the length
        Dim Length As Double = minlength + NormalizedVelocity * (maxlength - minlength)

        Return Length

    End Function

    Function GetWidth(velocity As Double, maxVelocity As Double, minWidth As Double, maxWidth As Double) As Double

        If velocity > 0 Then

            ' Normalize the velocity
            Dim NormalizedVelocity As Double = velocity / maxVelocity

            ' Interpolate the width
            Return minWidth + NormalizedVelocity * (maxWidth - minWidth)

        Else

            Return minWidth

        End If

    End Function

    Function GetReverseLength(velocity As Double, maxVelocity As Double, minlength As Double, maxlength As Double) As Double

        ' Reverse the velocity
        Dim ReversedVelocity As Double = -velocity

        ' Normalize the reversed velocity
        Dim NormalizedVelocity As Double = ReversedVelocity / maxVelocity

        ' Interpolate the length
        Dim Length As Double = minlength + NormalizedVelocity * (maxlength - minlength)

        Return Length

    End Function

    Function GetReverseWidth(velocity As Double, maxVelocity As Double, minWidth As Double, maxWidth As Double) As Double

        ' Reverse the velocity
        Dim ReversedVelocity As Double = -velocity

        If ReversedVelocity > 0 Then

            ' Normalize the reversed velocity
            Dim NormalizedVelocity As Double = ReversedVelocity / maxVelocity

            ' Interpolate the width
            Return minWidth + NormalizedVelocity * (maxWidth - minWidth)

        Else

            Return minWidth

        End If

    End Function

    Public Function DegreesToRadians(angleInDegrees As Single) As Single

        ' Convert degrees to radians by multiplying the angle by π / 180.
        Return angleInDegrees * (PI / 180)

        ' This formula converts degrees to radians because one full circle is
        ' 360 degrees or 2π radians, hence 1 degree equals π / 180 radians.
        ' π = 3.1415926535897931

    End Function

    Public Function ReverseAngleInRadians(angleInDegrees As Single) As Single

        AngleInRadians = DegreesToRadians(angleInDegrees)

        ' Calculate the reverse angle by adding π to the angle
        Return AngleInRadians + PI

    End Function

    ' Vectors and their opposites are widely used in fields like physics,
    ' engineering, computer graphics, and game development.

    ' Game Development: Vectors are essential for simulating movement,
    ' calculating trajectories, and handling collisions.

End Structure

Public Structure Body

    Public Brush As Brush

    Public Center As PointF

    Public Velocity As Double

    Public VelocityVector As PointF

    Public Acceleration As PointF

    Public MinVelocity As Integer

    Public MaxVelocity As Double

    Public AngleInDegrees As Single

    Public AngleInRadians As Single

    Public Width As Integer
    Public Height As Integer
    Dim HalfWidth As Integer
    Dim HalfHeight As Integer

    Private ReadOnly AlineCenterMiddle As StringFormat

    Dim KeyboardHints As PointF()

    Dim RotatedHints As PointF()

    Dim Body As PointF()

    Dim RotatedBody As PointF()

    Public HitBox As Rectangle

    Public Min As PointF ' Minimum corner
    Public Max As PointF ' Maximum corner

    Public KeyboardHintsFont As Font

    Public ShowKeyboardHints As Boolean

    Public ShowControllerHints As Boolean

    Public TimeToNextRotation As TimeSpan

    Private LastRotationTime As DateTime

    Private UnderglowPen As Pen

    Public ScaleFactor As Double

    Public Sub New(brush As Brush,
                   center As PointF,
                   width As Integer,
                   height As Integer,
                   angleInDegrees As Single, velocity As Double,
                   minVelocity As Double, maxVelocity As Double, acceleration As Double)


        UnderglowPen = New Pen(Color.FromArgb(200, Color.Blue), 4)

        AlineCenterMiddle = New StringFormat With {.Alignment = StringAlignment.Center,
                                                   .LineAlignment = StringAlignment.Center}

        TimeToNextRotation = TimeSpan.FromMilliseconds(1)

        LastRotationTime = Now

        Me.Velocity = velocity

        Me.MinVelocity = minVelocity

        Me.MaxVelocity = maxVelocity

        Me.Acceleration.X = acceleration
        Me.Acceleration.Y = acceleration

        Me.Center = center

        Me.Brush = brush

        Me.Width = width

        Me.Height = height

        HalfWidth = width / 2

        HalfHeight = height / 2

        ' Scale factor to make the length from nose to tail 128 pixels
        ScaleFactor = 1000 / 40.0 ' Previous length was approximately 40 pixels

        ScaleBody()
        ' Scale the body of the airplane based on the scale factor.

        ''Define an array of points representing an airplane.
        'Body = {
        '    New Point(CInt(3.5 * ScaleFactor), CInt(0 * ScaleFactor)), ' Nose
        '    New Point(CInt(3.4 * ScaleFactor), CInt(0.125 * ScaleFactor)),  ' Nose
        '    New Point(CInt(3.2 * ScaleFactor), CInt(0.125 * ScaleFactor)),  ' Nose
        '    New Point(CInt(3.1 * ScaleFactor), CInt(0.5 * ScaleFactor)),  ' Fuselage front
        '    New Point(CInt(2.5 * ScaleFactor), CInt(0.5 * ScaleFactor)),  ' Fuselage front
        '    New Point(CInt(2.25 * ScaleFactor), CInt(3 * ScaleFactor)),  ' Right wing root
        '    New Point(CInt(2 * ScaleFactor), CInt(3.5 * ScaleFactor)),  ' Right wing tip
        '    New Point(CInt(1.25 * ScaleFactor), CInt(3.5 * ScaleFactor)),  ' Right wing tip
        '    New Point(CInt(1 * ScaleFactor), CInt(3 * ScaleFactor)),  ' Right wing tip
        '    New Point(CInt(0.5 * ScaleFactor), CInt(0.5 * ScaleFactor)),  ' Right wing trailing edge
        '    New Point(CInt(-2 * ScaleFactor), CInt(0.25 * ScaleFactor)),  ' Right Fuselage rear
        '    New Point(CInt(-2.5 * ScaleFactor), CInt(1 * ScaleFactor)),  ' Right tail fin
        '    New Point(CInt(-3 * ScaleFactor), CInt(1 * ScaleFactor)),  ' Right tail fin
        '    New Point(CInt(-3.25 * ScaleFactor), CInt(0.125 * ScaleFactor)),  ' Right tail fin
        '    New Point(CInt(-3.5 * ScaleFactor), CInt(0 * ScaleFactor)),  ' Tail tip
        '    New Point(CInt(-3.25 * ScaleFactor), CInt(-0.125 * ScaleFactor)),  ' Left tail fin
        '    New Point(CInt(-3 * ScaleFactor), CInt(-1 * ScaleFactor)),  ' Left tail fin
        '    New Point(CInt(-2.5 * ScaleFactor), CInt(-1 * ScaleFactor)),  ' Left tail fin
        '    New Point(CInt(-2 * ScaleFactor), CInt(-0.25 * ScaleFactor)),  ' Left Fuselage rear
        '    New Point(CInt(0.5 * ScaleFactor), CInt(-0.5 * ScaleFactor)),  ' Left wing trailing edge
        '    New Point(CInt(1 * ScaleFactor), CInt(-3 * ScaleFactor)),  ' Left wing tip
        '    New Point(CInt(1.25 * ScaleFactor), CInt(-3.5 * ScaleFactor)),  ' Left wing tip
        '    New Point(CInt(2 * ScaleFactor), CInt(-3.5 * ScaleFactor)),  ' Left wing tip
        '    New Point(CInt(2.25 * ScaleFactor), CInt(-3 * ScaleFactor)),  ' Left wing root
        '    New Point(CInt(2.5 * ScaleFactor), CInt(-0.5 * ScaleFactor)),  ' Fuselage front
        '    New Point(CInt(3.1 * ScaleFactor), CInt(-0.5 * ScaleFactor)),  ' Fuselage front
        '    New Point(CInt(3.2 * ScaleFactor), CInt(-0.125 * ScaleFactor)),  ' Nose
        '    New Point(CInt(3.4 * ScaleFactor), CInt(-0.125 * ScaleFactor)),  ' Nose
        '    New Point(CInt(3.5 * ScaleFactor), CInt(0 * ScaleFactor)) ' Nose
        '    }

        RotatedBody = New PointF(Body.Length - 1) {}

        If angleInDegrees >= 0 AndAlso angleInDegrees <= 360 Then
            Me.AngleInDegrees = angleInDegrees
        Else
            Me.AngleInDegrees = 0
        End If

        AngleInRadians = DegreesToRadians(angleInDegrees)

        RotatedBody = RotatePoints(Body, center, AngleInRadians)



        KeyboardHintsFont = New Font("Segoe UI", 14, FontStyle.Bold)

        'KeyboardHints = {
        '    New PointF(HalfWidth - 10, -HalfHeight - 20),
        '    New PointF(HalfWidth - 10, HalfHeight + 20),
        '    New PointF(HalfWidth + 20, -HalfHeight + Me.Height / 2),
        '    New PointF(-HalfWidth - 20, -HalfHeight + Me.Height / 2)
        '}

        KeyboardHints = {
            New PointF(CInt(1.75 * ScaleFactor), CInt(-4.5 * ScaleFactor)), 'A
            New PointF(CInt(1.75 * ScaleFactor), CInt(4.5 * ScaleFactor)), 'D
            New PointF(CInt(4.5 * ScaleFactor), CInt(0 * ScaleFactor)),'W
            New PointF(CInt(-4.5 * ScaleFactor), CInt(0 * ScaleFactor))'S
        }

        RotatedHints = New PointF(KeyboardHints.Length - 1) {}

        RotatedHints = RotatePoints(KeyboardHints, center, AngleInRadians)

        ' Set velocity based on angle
        VelocityVector.X = Cos(AngleInRadians) * Me.Velocity
        VelocityVector.Y = Sin(AngleInRadians) * Me.Velocity

        ShowKeyboardHints = True

    End Sub

    Public Sub RotateClockwise()

        Dim ElapsedTime As TimeSpan = Now - LastRotationTime

        If ElapsedTime > TimeToNextRotation Then

            If AngleInDegrees < 360 Then

                AngleInDegrees += 1 ' Rotate clockwise

            Else

                AngleInDegrees = 0

            End If

            LastRotationTime = Now

        End If

    End Sub

    Public Sub RotateCounterClockwise()

        Dim ElapsedTime As TimeSpan = Now - LastRotationTime

        If ElapsedTime > TimeToNextRotation Then

            If AngleInDegrees > 0 Then

                AngleInDegrees -= 1 ' Rotate counterclockwise

            Else

                AngleInDegrees = 360

            End If

            LastRotationTime = Now

        End If

    End Sub

    Public Sub EmergencyStop(deltaTime As TimeSpan)

        If Velocity < 0 Then

            ' Calculate potential new velocity
            Dim newVelocity As Double = Velocity + (Acceleration.Y * 8 * deltaTime.TotalSeconds)

            If newVelocity > 0 Then

                Velocity = 0

            Else

                Velocity = newVelocity

            End If

        End If

        If Velocity > 0 Then

            ' Calculate potential new velocity
            Dim newVelocity As Double = Velocity + (-Acceleration.Y * 8 * deltaTime.TotalSeconds)

            If newVelocity < 0 Then

                Velocity = 0

            Else

                Velocity = newVelocity

            End If

        End If

    End Sub

    Public Sub Decelerate(deltaTime As TimeSpan)

        If Velocity < 0 Then

            ' Calculate potential new velocity
            Dim newVelocity As Double = Velocity + (Acceleration.Y * deltaTime.TotalSeconds)

            If newVelocity > MinVelocity Then

                Velocity = MinVelocity

            Else

                Velocity = newVelocity

            End If

        End If

        If Velocity > 0 Then

            ' Calculate potential new velocity
            Dim newVelocity As Double = Velocity + (-Acceleration.Y * deltaTime.TotalSeconds)

            If newVelocity < MinVelocity Then

                Velocity = MinVelocity

            Else

                Velocity = newVelocity

            End If

        End If

    End Sub

    Public Sub Update(ByVal deltaTime As TimeSpan)

        ScaleBody()

        KeyboardHints = {
            New PointF(CInt(1.75 * ScaleFactor), CInt(-4.5 * ScaleFactor)), 'A
            New PointF(CInt(1.75 * ScaleFactor), CInt(4.5 * ScaleFactor)), 'D
            New PointF(CInt(4.5 * ScaleFactor), CInt(0 * ScaleFactor)),'W
            New PointF(CInt(-4.5 * ScaleFactor), CInt(0 * ScaleFactor))'S
        }


        AngleInRadians = DegreesToRadians(AngleInDegrees)

        RotatedBody = RotatePoints(Body, Center, AngleInRadians)

        RotatedHints = RotatePoints(KeyboardHints, Center, AngleInRadians)

        ' Set velocity based on angle
        VelocityVector.X = Cos(AngleInRadians) * Velocity
        VelocityVector.Y = Sin(AngleInRadians) * Velocity

        UpdateMovement(deltaTime)

    End Sub

    Private Sub ScaleBody()
        ' Scale the body of the airplane based on the scale factor.

        'Define an array of points representing an airplane.
        Body = {
            New Point(CInt(3.5 * ScaleFactor), CInt(0 * ScaleFactor)),          ' Nose Start
            New Point(CInt(3.4 * ScaleFactor), CInt(0.125 * ScaleFactor)),      ' Nose
            New Point(CInt(3.2 * ScaleFactor), CInt(0.125 * ScaleFactor)),      ' Nose
            New Point(CInt(3.1 * ScaleFactor), CInt(0.5 * ScaleFactor)),        ' Fuselage front
            New Point(CInt(2.5 * ScaleFactor), CInt(0.5 * ScaleFactor)),        ' Fuselage front
            New Point(CInt(2.25 * ScaleFactor), CInt(3 * ScaleFactor)),         ' Right wing root
            New Point(CInt(2 * ScaleFactor), CInt(3.5 * ScaleFactor)),          ' Right wing tip
            New Point(CInt(1.25 * ScaleFactor), CInt(3.5 * ScaleFactor)),       ' Right wing tip
            New Point(CInt(1 * ScaleFactor), CInt(3 * ScaleFactor)),            ' Right wing tip
            New Point(CInt(0.5 * ScaleFactor), CInt(0.5 * ScaleFactor)),        ' Right wing trailing edge
            New Point(CInt(-2 * ScaleFactor), CInt(0.25 * ScaleFactor)),        ' Right Fuselage rear
            New Point(CInt(-2.5 * ScaleFactor), CInt(1 * ScaleFactor)),         ' Right tail fin
            New Point(CInt(-3 * ScaleFactor), CInt(1 * ScaleFactor)),           ' Right tail fin
            New Point(CInt(-3.25 * ScaleFactor), CInt(0.125 * ScaleFactor)),    ' Right tail fin
            New Point(CInt(-3.5 * ScaleFactor), CInt(0 * ScaleFactor)),         ' Tail tip
            New Point(CInt(-3.25 * ScaleFactor), CInt(-0.125 * ScaleFactor)),   ' Left tail fin
            New Point(CInt(-3 * ScaleFactor), CInt(-1 * ScaleFactor)),          ' Left tail fin
            New Point(CInt(-2.5 * ScaleFactor), CInt(-1 * ScaleFactor)),        ' Left tail fin
            New Point(CInt(-2 * ScaleFactor), CInt(-0.25 * ScaleFactor)),       ' Left Fuselage rear
            New Point(CInt(0.5 * ScaleFactor), CInt(-0.5 * ScaleFactor)),       ' Left wing trailing edge
            New Point(CInt(1 * ScaleFactor), CInt(-3 * ScaleFactor)),           ' Left wing tip
            New Point(CInt(1.25 * ScaleFactor), CInt(-3.5 * ScaleFactor)),      ' Left wing tip
            New Point(CInt(2 * ScaleFactor), CInt(-3.5 * ScaleFactor)),         ' Left wing tip
            New Point(CInt(2.25 * ScaleFactor), CInt(-3 * ScaleFactor)),        ' Left wing root
            New Point(CInt(2.5 * ScaleFactor), CInt(-0.5 * ScaleFactor)),       ' Fuselage front
            New Point(CInt(3.1 * ScaleFactor), CInt(-0.5 * ScaleFactor)),       ' Fuselage front
            New Point(CInt(3.2 * ScaleFactor), CInt(-0.125 * ScaleFactor)),     ' Nose
            New Point(CInt(3.4 * ScaleFactor), CInt(-0.125 * ScaleFactor)),     ' Nose
            New Point(CInt(3.5 * ScaleFactor), CInt(0 * ScaleFactor))           ' Nose End
            }

    End Sub

    Public Sub Draw(g As Graphics)

        'g?.FillEllipse(Brushes.LightGray, Center.X - 72, Center.Y - 72, 144, 144)

        ' Draw underglow.
        g?.DrawPolygon(UnderglowPen, RotatedBody)

        FillBodyWithLinearGradient(g)

        DrawHints(g)

    End Sub

    Private Sub FillBodyWithLinearGradient(g As Graphics)

        ' Define the gradient brush with a larger virtual space
        Dim GradientRectangle As New RectangleF(Center.X - 300, Center.Y - 300, 512, 512) ' Width and height control the texture size


        ' Define the virtual space for the gradient
        Dim DiagonalGradientBrush As New LinearGradientBrush(GradientRectangle,
                                                       Color.White,
                                                       Color.Black,
                                          LinearGradientMode.ForwardDiagonal)

        g?.FillPolygon(DiagonalGradientBrush, RotatedBody)

    End Sub

    Private Sub DrawHints(g As Graphics)

        If ShowKeyboardHints Then

            g?.FillEllipse(Brushes.Black, RotatedHints(0).X - 17, RotatedHints(0).Y - 17, 34, 34)
            g?.DrawString("A", KeyboardHintsFont, Brushes.White, RotatedHints(0), AlineCenterMiddle)

            g?.FillEllipse(Brushes.Black, RotatedHints(1).X - 17, RotatedHints(1).Y - 17, 34, 34)
            g?.DrawString("D", KeyboardHintsFont, Brushes.White, RotatedHints(1), AlineCenterMiddle)

            g?.FillEllipse(Brushes.Black, RotatedHints(2).X - 17, RotatedHints(2).Y - 17, 34, 34)
            g?.DrawString("W", KeyboardHintsFont, Brushes.White, RotatedHints(2), AlineCenterMiddle)

            g?.FillEllipse(Brushes.Black, RotatedHints(3).X - 17, RotatedHints(3).Y - 17, 34, 34)
            g?.DrawString("S", KeyboardHintsFont, Brushes.White, RotatedHints(3), AlineCenterMiddle)

        End If

        If ShowControllerHints Then

            g?.FillEllipse(Brushes.Black, RotatedHints(0).X - 17, RotatedHints(0).Y - 17, 34, 34)
            g?.DrawString("L", KeyboardHintsFont, Brushes.White, RotatedHints(0), AlineCenterMiddle)

            g?.FillEllipse(Brushes.Black, RotatedHints(1).X - 17, RotatedHints(1).Y - 17, 34, 34)
            g?.DrawString("R", KeyboardHintsFont, Brushes.White, RotatedHints(1), AlineCenterMiddle)

            g?.FillEllipse(Brushes.Black, RotatedHints(2).X - 17, RotatedHints(2).Y - 17, 34, 34)
            g?.DrawString("A", KeyboardHintsFont, Brushes.White, RotatedHints(2), AlineCenterMiddle)

            g?.FillEllipse(Brushes.Black, RotatedHints(3).X - 17, RotatedHints(3).Y - 17, 34, 34)
            g?.DrawString("Y", KeyboardHintsFont, Brushes.White, RotatedHints(3), AlineCenterMiddle)

        End If

    End Sub

    Public Sub UpdateMovement(ByVal deltaTime As TimeSpan)

        'Move horizontally.
        Center.X += CSng(VelocityVector.X * deltaTime.TotalSeconds) 'Δs = V * Δt
        'Displacement = Velocity x Delta Time

        'Move verticaly.
        Center.Y += CSng(VelocityVector.Y * deltaTime.TotalSeconds) 'Δs = V * Δt
        'Displacement = Velocity x Delta Time

    End Sub






    Public Sub CheckWraparound(points As PointF(), clientWidth As Integer, clientHeight As Integer)
        ' Function to check for wraparound and adjust position of the body.

        ' Check for collision with the left and right boundaries.
        If Center.X < 0 Then
            Center.X = clientWidth
        ElseIf Center.X > clientWidth Then
            Center.X = 0
        End If

        ' Check for collision with the top and bottom boundaries.
        If Center.Y < 0 Then
            Center.Y = clientHeight
        ElseIf Center.Y > clientHeight Then
            Center.Y = 0
        End If
    End Sub









    Public Sub CheckWallBounce(points As PointF(), clientWidth As Integer, clientHeight As Integer)
        ' Function to check for wall bounce and adjust position and velocity of the body.

        ' Check for collision with the left and right boundaries.
        If Center.X <= 0 Then

            Center.X = 0

            ' Reverse velocity for bounce back.
            Velocity = -Velocity

        ElseIf Center.X >= clientWidth Then

            Center.X = clientWidth

            ' Reverse velocity for bounce back.
            Velocity = -Velocity

        End If

        ' Check for collision with the top and bottom boundaries
        If Center.Y <= 0 Then

            Center.Y = 0

            ' Reverse velocity for bounce back.
            Velocity = -Velocity

        ElseIf Center.Y >= clientHeight Then

            Center.Y = clientHeight

            ' Reverse velocity for bounce back.
            Velocity = -Velocity

        End If

    End Sub

    Private Function RotatePoints(points As PointF(), center As PointF, angleInRadians As Single) As PointF()

        Dim RotatedPoints As PointF()

        RotatedPoints = New PointF(points.Length - 1) {}

        For i As Integer = 0 To points.Length - 1

            Dim x As Single = points(i).X * Cos(angleInRadians) - points(i).Y * Sin(angleInRadians)
            Dim y As Single = points(i).X * Sin(angleInRadians) + points(i).Y * Cos(angleInRadians)

            RotatedPoints(i) = New PointF(x + center.X, y + center.Y)

        Next

        Return RotatedPoints

    End Function

    Public Function DegreesToRadians(AngleInDegrees As Single)

        DegreesToRadians = AngleInDegrees * (PI / 180)

    End Function

    ' Method to check if a point is inside the AABB
    Public Function Contains(point As PointF) As Boolean
        Return point.X >= Min.X AndAlso point.X <= Max.X AndAlso
               point.Y >= Min.Y AndAlso point.Y <= Max.Y
    End Function

    Public Function Intersects(rectangle As Rectangle) As Boolean
        ' Function to check if the rectangle intersects with the points

        ' Get the minimum and maximum points of the body
        GetMinMax(Body)

        ' Return if there is an intersection
        Return Min.X <= rectangle.Right AndAlso Max.X >= rectangle.Left AndAlso
           Min.Y <= rectangle.Bottom AndAlso Max.Y >= rectangle.Top

    End Function

    ' Subroutine to get the minimum and maximum points
    Private Sub GetMinMax(points As PointF())

        ' Initialize Min and Max to the first point
        Min = points(0)
        Max = points(0)

        ' Iterate through the points to find the min and max
        For Each point As PointF In points
            If point.X < Min.X Then Min.X = point.X
            If point.Y < Min.Y Then Min.Y = point.Y
            If point.X > Max.X Then Max.X = point.X
            If point.Y > Max.Y Then Max.Y = point.Y
        Next

    End Sub

End Structure

Public Structure AudioPlayer

    <DllImport("winmm.dll", EntryPoint:="mciSendStringW")>
    Private Shared Function mciSendStringW(<MarshalAs(UnmanagedType.LPWStr)> ByVal lpszCommand As String,
                                           <MarshalAs(UnmanagedType.LPWStr)> ByVal lpszReturnString As StringBuilder,
                                           ByVal cchReturn As UInteger, ByVal hwndCallback As IntPtr) As Integer
    End Function

    Private Sounds() As String

    Public Function AddSound(SoundName As String, FilePath As String) As Boolean

        ' Do we have a name and does the file exist?
        If Not String.IsNullOrWhiteSpace(SoundName) AndAlso IO.File.Exists(FilePath) Then
            ' Yes, we have a name and the file exists.

            Dim CommandOpen As String = $"open ""{FilePath}"" alias {SoundName}"

            ' Do we have sounds?
            If Sounds Is Nothing Then
                ' No we do not have sounds.

                ' Did the sound file open?
                If SendMciCommand(CommandOpen, IntPtr.Zero) Then
                    ' Yes, the sound file did open.

                    ' Start the Sounds array with the sound.
                    ReDim Sounds(0)

                    Sounds(0) = SoundName

                    Return True ' The sound was added.

                End If

                ' Is the sound in the array already?
            ElseIf Not Sounds.Contains(SoundName) Then
                ' Yes we have sounds and no the sound is not in the array.

                ' Did the sound file open?
                If SendMciCommand(CommandOpen, IntPtr.Zero) Then
                    ' Yes, the sound file did open.

                    ' Add the sound to the Sounds array.
                    Array.Resize(Sounds, Sounds.Length + 1)

                    Sounds(Sounds.Length - 1) = SoundName

                    Return True ' The sound was added.

                End If

            End If

        End If

        Debug.Print($"{SoundName} not added to sounds.")

        Return False ' The sound was not added.

    End Function

    Public Function SetVolume(SoundName As String, Level As Integer) As Boolean

        ' Do we have sounds and is the sound in the array and is the level in the valid range?
        If Sounds IsNot Nothing AndAlso Sounds.Contains(SoundName) AndAlso Level >= 0 AndAlso Level <= 1000 Then
            ' We have sounds and the sound is in the array and the level is in range.

            Dim CommandVolume As String = $"setaudio {SoundName} volume to {Level}"

            Return SendMciCommand(CommandVolume, IntPtr.Zero) ' The volume was set.

        End If

        Debug.Print($"{SoundName} volume not set.")

        Return False ' The volume was not set.

    End Function

    Public Function LoopSound(SoundName As String) As Boolean

        ' Do we have sounds and is the sound in the array?
        If Sounds IsNot Nothing AndAlso Sounds.Contains(SoundName) Then
            ' We have sounds and the sound is in the array.

            Dim CommandSeekToStart As String = $"seek {SoundName} to start"

            Dim CommandPlayRepeat As String = $"play {SoundName} repeat"

            Return SendMciCommand(CommandSeekToStart, IntPtr.Zero) AndAlso
                   SendMciCommand(CommandPlayRepeat, IntPtr.Zero) ' The sound is looping.

        End If

        Debug.Print($"{SoundName} not looping.")

        Return False ' The sound is not looping.

    End Function

    Public Function PlaySound(SoundName As String) As Boolean

        ' Do we have sounds and is the sound in the array?
        If Sounds IsNot Nothing AndAlso Sounds.Contains(SoundName) Then
            ' We have sounds and the sound is in the array.

            Dim CommandSeekToStart As String = $"seek {SoundName} to start"

            Dim CommandPlay As String = $"play {SoundName} notify"

            Return SendMciCommand(CommandSeekToStart, IntPtr.Zero) AndAlso
                   SendMciCommand(CommandPlay, IntPtr.Zero) ' The sound is playing.

        End If

        Debug.Print($"{SoundName} not playing.")

        Return False ' The sound is not playing.

    End Function

    Public Function PauseSound(SoundName As String) As Boolean

        ' Do we have sounds and is the sound in the array?
        If Sounds IsNot Nothing AndAlso Sounds.Contains(SoundName) Then
            ' We have sounds and the sound is in the array.

            Dim CommandPause As String = $"pause {SoundName} notify"

            Return SendMciCommand(CommandPause, IntPtr.Zero) ' The sound is paused.

        End If

        Debug.Print($"{SoundName} not paused.")

        Return False ' The sound is not paused.

    End Function

    Public Function IsPlaying(SoundName As String) As Boolean

        Return GetStatus(SoundName, "mode") = "playing"

    End Function

    Public Sub AddOverlapping(SoundName As String, FilePath As String)

        For Each Suffix As String In {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L"}

            AddSound(SoundName & Suffix, FilePath)

        Next

    End Sub

    Public Sub PlayOverlapping(SoundName As String)

        For Each Suffix As String In {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L"}

            If Not IsPlaying(SoundName & Suffix) Then

                PlaySound(SoundName & Suffix)

                Exit Sub

            End If

        Next

    End Sub

    Public Sub SetVolumeOverlapping(SoundName As String, Level As Integer)

        For Each Suffix As String In {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L"}

            SetVolume(SoundName & Suffix, Level)

        Next

    End Sub

    Private Function SendMciCommand(command As String, hwndCallback As IntPtr) As Boolean

        Dim ReturnString As New StringBuilder(128)

        Try

            Return mciSendStringW(command, ReturnString, 0, hwndCallback) = 0

        Catch ex As Exception

            Debug.Print($"Error sending MCI command: {command} | {ex.Message}")

            Return False

        End Try

    End Function

    Private Function GetStatus(SoundName As String, StatusType As String) As String

        Try

            ' Do we have sounds and is the sound in the array?
            If Sounds IsNot Nothing AndAlso Sounds.Contains(SoundName) Then
                ' We have sounds and the sound is in the array.

                Dim CommandStatus As String = $"status {SoundName} {StatusType}"

                Dim StatusReturn As New StringBuilder(128)

                mciSendStringW(CommandStatus, StatusReturn, 128, IntPtr.Zero)

                Return StatusReturn.ToString.Trim.ToLower

            End If

        Catch ex As Exception

            Debug.Print($"Error getting status: {SoundName} | {ex.Message}")

        End Try

        Return String.Empty

    End Function

    Public Sub CloseSounds()

        If Sounds IsNot Nothing Then

            For Each Sound In Sounds

                Dim CommandClose As String = $"close {Sound}"

                SendMciCommand(CommandClose, IntPtr.Zero)

            Next

            Sounds = Nothing

        End If

    End Sub

End Structure

Public Structure XboxControllers

    <DllImport("XInput1_4.dll")>
    Private Shared Function XInputGetState(dwUserIndex As Integer,
                                     ByRef pState As XINPUT_STATE) As Integer
    End Function

    <StructLayout(LayoutKind.Explicit)>
    Private Structure XINPUT_STATE

        <FieldOffset(0)>
        Public dwPacketNumber As UInteger ' Unsigned integer range 0 through 4,294,967,295.
        <FieldOffset(4)>
        Public Gamepad As XINPUT_GAMEPAD
    End Structure

    <StructLayout(LayoutKind.Sequential)>
    Private Structure XINPUT_GAMEPAD

        Public wButtons As UShort ' Unsigned integer range 0 through 65,535.
        Public bLeftTrigger As Byte ' Unsigned integer range 0 through 255.
        Public bRightTrigger As Byte
        Public sThumbLX As Short ' Signed integer range -32,768 through 32,767.
        Public sThumbLY As Short
        Public sThumbRX As Short
        Public sThumbRY As Short
    End Structure

    Private State As XINPUT_STATE

    Private Enum Button

        DPadUp = 1
        DPadDown = 2
        DPadLeft = 4
        DPadRight = 8
        Start = 16
        Back = 32
        LeftStick = 64
        RightStick = 128
        LeftBumper = 256
        RightBumper = 512
        A = 4096
        B = 8192
        X = 16384
        Y = 32768
    End Enum

    ' Set the start of the thumbstick neutral zone to 1/2 over.
    Private Const NeutralStart As Short = -16384 ' -16,384 = -32,768 / 2
    ' The thumbstick position must be more than 1/2 over the neutral start to
    ' register as moved.
    ' A short is a signed 16-bit (2-byte) integer range -32,768 through 32,767.
    ' This gives us 65,536 values.

    ' Set the end of the thumbstick neutral zone to 1/2 over.
    Private Const NeutralEnd As Short = 16384 ' 16,383.5 = 32,767 / 2
    ' The thumbstick position must be more than 1/2 over the neutral end to
    ' register as moved.

    ' Set the trigger threshold to 1/4 pull.
    Private Const TriggerThreshold As Byte = 64 ' 64 = 256 / 4
    ' The trigger position must be greater than the trigger threshold to
    ' register as pulled.
    ' A byte is a unsigned 8-bit (1-byte) integer range 0 through 255.
    ' This gives us 256 values.

    Public Connected() As Boolean

    Private ConnectionStart As Date

    Public Buttons() As UShort

    Public LeftThumbstickXaxisNeutral() As Boolean
    Public LeftThumbstickYaxisNeutral() As Boolean

    Public RightThumbstickXaxisNeutral() As Boolean
    Public RightThumbstickYaxisNeutral() As Boolean

    Public DPadNeutral() As Boolean

    Public LetterButtonsNeutral() As Boolean

    Public DPadUp() As Boolean
    Public DPadDown() As Boolean
    Public DPadLeft() As Boolean
    Public DPadRight() As Boolean

    Public Start() As Boolean
    Public Back() As Boolean

    Public LeftStick() As Boolean
    Public RightStick() As Boolean

    Public LeftBumper() As Boolean
    Public RightBumper() As Boolean

    Public A() As Boolean
    Public B() As Boolean
    Public X() As Boolean
    Public Y() As Boolean

    Public RightThumbstickUp() As Boolean
    Public RightThumbstickDown() As Boolean
    Public RightThumbstickLeft() As Boolean
    Public RightThumbstickRight() As Boolean

    Public LeftThumbstickUp() As Boolean
    Public LeftThumbstickDown() As Boolean
    Public LeftThumbstickLeft() As Boolean
    Public LeftThumbstickRight() As Boolean

    Public LeftTrigger() As Boolean
    Public RightTrigger() As Boolean

    Public TimeToVibe As Integer

    Private LeftVibrateStart() As Date

    Private RightVibrateStart() As Date

    Private IsLeftVibrating() As Boolean

    Private IsRightVibrating() As Boolean

    Public Sub Initialize()

        ' Initialize the Connected array to indicate whether controllers are connected.
        Connected = New Boolean(0 To 3) {}

        ' Record the current date and time when initialization starts.
        ConnectionStart = DateTime.Now

        ' Initialize the Buttons array to store the state of controller buttons.
        Buttons = New UShort(0 To 3) {}

        ' Initialize arrays to check if thumbstick axes are in the neutral position.
        LeftThumbstickXaxisNeutral = New Boolean(0 To 3) {}
        LeftThumbstickYaxisNeutral = New Boolean(0 To 3) {}
        RightThumbstickXaxisNeutral = New Boolean(0 To 3) {}
        RightThumbstickYaxisNeutral = New Boolean(0 To 3) {}

        ' Initialize array to check if the D-Pad is in the neutral position.
        DPadNeutral = New Boolean(0 To 3) {}

        ' Initialize array to check if letter buttons are in the neutral position.
        LetterButtonsNeutral = New Boolean(0 To 3) {}

        ' Set all thumbstick axes, triggers, D-Pad, letter buttons, start/back buttons,
        ' bumpers,and stick buttons to neutral for all controllers (indices 0 to 3).
        For i As Integer = 0 To 3

            LeftThumbstickXaxisNeutral(i) = True
            LeftThumbstickYaxisNeutral(i) = True
            RightThumbstickXaxisNeutral(i) = True
            RightThumbstickYaxisNeutral(i) = True

            DPadNeutral(i) = True

            LetterButtonsNeutral(i) = True

        Next

        ' Initialize arrays for thumbstick directional states.
        RightThumbstickLeft = New Boolean(0 To 3) {}
        RightThumbstickRight = New Boolean(0 To 3) {}
        RightThumbstickDown = New Boolean(0 To 3) {}
        RightThumbstickUp = New Boolean(0 To 3) {}
        LeftThumbstickLeft = New Boolean(0 To 3) {}
        LeftThumbstickRight = New Boolean(0 To 3) {}
        LeftThumbstickDown = New Boolean(0 To 3) {}
        LeftThumbstickUp = New Boolean(0 To 3) {}

        ' Initialize arrays for trigger states.
        LeftTrigger = New Boolean(0 To 3) {}
        RightTrigger = New Boolean(0 To 3) {}

        ' Initialize arrays for letter button states (A, B, X, Y).
        A = New Boolean(0 To 3) {}
        B = New Boolean(0 To 3) {}
        X = New Boolean(0 To 3) {}
        Y = New Boolean(0 To 3) {}

        ' Initialize arrays for bumper button states.
        LeftBumper = New Boolean(0 To 3) {}
        RightBumper = New Boolean(0 To 3) {}

        ' Initialize arrays for D-Pad directional states.
        DPadUp = New Boolean(0 To 3) {}
        DPadDown = New Boolean(0 To 3) {}
        DPadLeft = New Boolean(0 To 3) {}
        DPadRight = New Boolean(0 To 3) {}

        ' Initialize arrays for start and back button states.
        Start = New Boolean(0 To 3) {}
        Back = New Boolean(0 To 3) {}

        ' Initialize arrays for stick button states.
        LeftStick = New Boolean(0 To 3) {}
        RightStick = New Boolean(0 To 3) {}

        TimeToVibe = 1000 'ms

        LeftVibrateStart = New Date(0 To 3) {}
        RightVibrateStart = New Date(0 To 3) {}

        For ControllerNumber As Integer = 0 To 3

            LeftVibrateStart(ControllerNumber) = Now

            RightVibrateStart(ControllerNumber) = Now

        Next

        IsLeftVibrating = New Boolean(0 To 3) {}
        IsRightVibrating = New Boolean(0 To 3) {}

        ' Call the TestInitialization method to verify the initial state of the controllers.
        TestInitialization()

    End Sub

    Public Sub Update()

        Dim ElapsedTime As TimeSpan = Now - ConnectionStart

        ' Every second check for connected controllers.
        If ElapsedTime.TotalSeconds >= 1 Then

            For ControllerNumber As Integer = 0 To 3 ' Up to 4 controllers

                Connected(ControllerNumber) = IsConnected(ControllerNumber)

            Next

            ConnectionStart = DateTime.Now

        End If

        For ControllerNumber As Integer = 0 To 3

            If Connected(ControllerNumber) Then

                UpdateState(ControllerNumber)

            End If

        Next

        UpdateVibrateTimers()

    End Sub

    Private Sub UpdateState(controllerNumber As Integer)

        Try

            XInputGetState(controllerNumber, State)

            UpdateButtons(controllerNumber)

            UpdateThumbsticks(controllerNumber)

            UpdateTriggers(controllerNumber)

        Catch ex As Exception
            ' Something went wrong (An exception occurred).

            Debug.Print($"Error getting XInput state: {controllerNumber} | {ex.Message}")

        End Try

    End Sub

    Private Sub UpdateButtons(CID As Integer)

        UpdateDPadButtons(CID)

        UpdateLetterButtons(CID)

        UpdateBumperButtons(CID)

        UpdateStickButtons(CID)

        UpdateStartBackButtons(CID)

        UpdateDPadNeutral(CID)

        UpdateLetterButtonsNeutral(CID)

        Buttons(CID) = State.Gamepad.wButtons

    End Sub

    Private Sub UpdateThumbsticks(controllerNumber As Integer)

        UpdateLeftThumbstick(controllerNumber)

        UpdateRightThumbstick(controllerNumber)

    End Sub

    Private Sub UpdateTriggers(controllerNumber As Integer)

        UpdateLeftTrigger(controllerNumber)

        UpdateRightTrigger(controllerNumber)

    End Sub

    Private Sub UpdateDPadButtons(CID As Integer)

        DPadUp(CID) = (State.Gamepad.wButtons And Button.DPadUp) <> 0
        DPadDown(CID) = (State.Gamepad.wButtons And Button.DPadDown) <> 0
        DPadLeft(CID) = (State.Gamepad.wButtons And Button.DPadLeft) <> 0
        DPadRight(CID) = (State.Gamepad.wButtons And Button.DPadRight) <> 0

    End Sub

    Private Sub UpdateLetterButtons(CID As Integer)

        A(CID) = (State.Gamepad.wButtons And Button.A) <> 0
        B(CID) = (State.Gamepad.wButtons And Button.B) <> 0
        X(CID) = (State.Gamepad.wButtons And Button.X) <> 0
        Y(CID) = (State.Gamepad.wButtons And Button.Y) <> 0

    End Sub

    Private Sub UpdateBumperButtons(CID As Integer)

        LeftBumper(CID) = (State.Gamepad.wButtons And Button.LeftBumper) <> 0
        RightBumper(CID) = (State.Gamepad.wButtons And Button.RightBumper) <> 0

    End Sub

    Private Sub UpdateStickButtons(CID As Integer)

        LeftStick(CID) = (State.Gamepad.wButtons And Button.LeftStick) <> 0
        RightStick(CID) = (State.Gamepad.wButtons And Button.RightStick) <> 0

    End Sub

    Private Sub UpdateStartBackButtons(CID As Integer)

        Start(CID) = (State.Gamepad.wButtons And Button.Start) <> 0
        Back(CID) = (State.Gamepad.wButtons And Button.Back) <> 0

    End Sub

    Private Sub UpdateLeftThumbstick(ControllerNumber As Integer)

        UpdateLeftThumbstickXaxis(ControllerNumber)

        UpdateLeftThumbstickYaxis(ControllerNumber)

    End Sub

    Private Sub UpdateLeftThumbstickYaxis(ControllerNumber As Integer)
        ' The range on the Y-axis is -32,768 through 32,767.
        ' Signed 16-bit (2-byte) integer.

        ' What position is the left thumbstick in on the Y-axis?
        If State.Gamepad.sThumbLY <= NeutralStart Then
            ' The left thumbstick is in the down position.

            LeftThumbstickUp(ControllerNumber) = False

            LeftThumbstickYaxisNeutral(ControllerNumber) = False

            LeftThumbstickDown(ControllerNumber) = True

        ElseIf State.Gamepad.sThumbLY >= NeutralEnd Then
            ' The left thumbstick is in the up position.

            LeftThumbstickDown(ControllerNumber) = False

            LeftThumbstickYaxisNeutral(ControllerNumber) = False

            LeftThumbstickUp(ControllerNumber) = True

        Else
            ' The left thumbstick is in the neutral position.

            LeftThumbstickUp(ControllerNumber) = False

            LeftThumbstickDown(ControllerNumber) = False

            LeftThumbstickYaxisNeutral(ControllerNumber) = True

        End If

    End Sub

    Private Sub UpdateLeftThumbstickXaxis(ControllerNumber As Integer)
        ' The range on the X-axis is -32,768 through 32,767.
        ' sThumbLX is a signed 16-bit (2-byte) integer.

        ' What position is the left thumbstick in on the X-axis?
        If State.Gamepad.sThumbLX <= NeutralStart Then
            ' The left thumbstick is in the left position.

            LeftThumbstickRight(ControllerNumber) = False

            LeftThumbstickXaxisNeutral(ControllerNumber) = False

            LeftThumbstickLeft(ControllerNumber) = True

        ElseIf State.Gamepad.sThumbLX >= NeutralEnd Then
            ' The left thumbstick is in the right position.

            LeftThumbstickLeft(ControllerNumber) = False

            LeftThumbstickXaxisNeutral(ControllerNumber) = False

            LeftThumbstickRight(ControllerNumber) = True

        Else
            ' The left thumbstick is in the neutral position.

            LeftThumbstickLeft(ControllerNumber) = False

            LeftThumbstickRight(ControllerNumber) = False

            LeftThumbstickXaxisNeutral(ControllerNumber) = True

        End If

    End Sub

    Private Sub UpdateRightThumbstick(ControllerNumber As Integer)

        UpdateRightThumbstickXaxis(ControllerNumber)

        UpdateRightThumbstickYaxis(ControllerNumber)

    End Sub

    Private Sub UpdateRightThumbstickYaxis(ControllerNumber As Integer)
        ' The range on the Y-axis is -32,768 through 32,767.
        ' sThumbRY is a signed 16-bit (2-byte) integer.

        ' What position is the right thumbstick in on the Y-axis?
        If State.Gamepad.sThumbRY <= NeutralStart Then
            ' The right thumbstick is in the down position.

            RightThumbstickUp(ControllerNumber) = False

            RightThumbstickYaxisNeutral(ControllerNumber) = False

            RightThumbstickDown(ControllerNumber) = True

        ElseIf State.Gamepad.sThumbRY >= NeutralEnd Then
            ' The right thumbstick is in the up position.

            RightThumbstickDown(ControllerNumber) = False

            RightThumbstickYaxisNeutral(ControllerNumber) = False

            RightThumbstickUp(ControllerNumber) = True

        Else
            ' The right thumbstick is in the neutral position.

            RightThumbstickUp(ControllerNumber) = False

            RightThumbstickDown(ControllerNumber) = False

            RightThumbstickYaxisNeutral(ControllerNumber) = True

        End If

    End Sub

    Private Sub UpdateRightThumbstickXaxis(ControllerNumber As Integer)
        ' The range on the X-axis is -32,768 through 32,767.
        ' Signed 16-bit (2-byte) integer.

        ' What position is the right thumbstick in on the X-axis?
        If State.Gamepad.sThumbRX <= NeutralStart Then
            ' The right thumbstick is in the left position.

            RightThumbstickRight(ControllerNumber) = False

            RightThumbstickXaxisNeutral(ControllerNumber) = False

            RightThumbstickLeft(ControllerNumber) = True

        ElseIf State.Gamepad.sThumbRX >= NeutralEnd Then
            ' The right thumbstick is in the right position.

            RightThumbstickLeft(ControllerNumber) = False

            RightThumbstickXaxisNeutral(ControllerNumber) = False

            RightThumbstickRight(ControllerNumber) = True

        Else
            ' The right thumbstick is in the neutral position.

            RightThumbstickLeft(ControllerNumber) = False

            RightThumbstickRight(ControllerNumber) = False

            RightThumbstickXaxisNeutral(ControllerNumber) = True

        End If

    End Sub

    Private Sub UpdateRightTrigger(ControllerNumber As Integer)
        ' The range of right trigger is 0 to 255. Unsigned 8-bit (1-byte) integer.
        ' The trigger position must be greater than the trigger threshold to
        ' register as pressed.

        ' What position is the right trigger in?
        If State.Gamepad.bRightTrigger > TriggerThreshold Then
            ' The right trigger is in the down position. Trigger Break. Bang!

            RightTrigger(ControllerNumber) = True

        Else
            ' The right trigger is in the neutral position. Pre-Travel.

            RightTrigger(ControllerNumber) = False

        End If

    End Sub

    Private Sub UpdateLeftTrigger(ControllerNumber As Integer)
        ' The range of left trigger is 0 to 255. Unsigned 8-bit (1-byte) integer.
        ' The trigger position must be greater than the trigger threshold to
        ' register as pressed.

        ' What position is the left trigger in?
        If State.Gamepad.bLeftTrigger > TriggerThreshold Then
            ' The left trigger is in the fire position. Trigger Break. Bang!

            LeftTrigger(ControllerNumber) = True

        Else
            ' The left trigger is in the neutral position. Pre-Travel.

            LeftTrigger(ControllerNumber) = False

        End If

    End Sub

    Private Sub UpdateDPadNeutral(controllerNumber As Integer)

        If DPadDown(controllerNumber) Or
           DPadLeft(controllerNumber) Or
           DPadRight(controllerNumber) Or
           DPadUp(controllerNumber) Then

            DPadNeutral(controllerNumber) = False

        Else

            DPadNeutral(controllerNumber) = True

        End If

    End Sub

    Private Sub UpdateLetterButtonsNeutral(controllerNumber As Integer)

        If A(controllerNumber) Or
           B(controllerNumber) Or
           X(controllerNumber) Or
           Y(controllerNumber) Then

            LetterButtonsNeutral(controllerNumber) = False

        Else

            LetterButtonsNeutral(controllerNumber) = True

        End If

    End Sub

    Private Function IsConnected(controllerNumber As Integer) As Boolean

        Try

            Return XInputGetState(controllerNumber, State) = 0
            ' 0 means the controller is connected.
            ' Anything else (a non-zero value) means the controller is not
            ' connected.

        Catch ex As Exception
            ' Something went wrong (An exception occured).

            Debug.Print($"Error getting XInput state: {controllerNumber} | {ex.Message}")

            Return False

        End Try

    End Function

    Private Sub TestInitialization()

        ' Check that ConnectionStart is not Nothing (initialization was successful)
        Debug.Assert(Not ConnectionStart = Nothing,
                     $"Connection Start should not be Nothing.")

        ' Check that Buttons array is initialized
        Debug.Assert(Buttons IsNot Nothing,
                     $"Buttons should not be Nothing.")

        Debug.Assert(Not TimeToVibe = Nothing,
                     $"TimeToVibe should not be Nothing.")

        For i As Integer = 0 To 3

            ' Check that all controllers are initialized as not connected
            Debug.Assert(Not Connected(i),
                         $"Controller {i} should not be connected after initialization.")

            ' Check that all axes of the Left Thumbsticks are initialized as neutral. 
            Debug.Assert(LeftThumbstickXaxisNeutral(i),
                         $"Left Thumbstick X-axis for Controller {i} should be neutral.")
            Debug.Assert(LeftThumbstickYaxisNeutral(i),
                         $"Left Thumbstick Y-axis for Controller {i} should be neutral.")

            ' Check that all axes of the Right Thumbsticks are initialized as neutral. 
            Debug.Assert(RightThumbstickXaxisNeutral(i),
                         $"Right Thumbstick X-axis for Controller {i} should be neutral.")
            Debug.Assert(RightThumbstickYaxisNeutral(i),
                         $"Right Thumbstick Y-axis for Controller {i} should be neutral.")

            ' Check that all DPads are initialized as neutral. 
            Debug.Assert(DPadNeutral(i),
                         $"DPad for Controller {i} should be neutral.")

            ' Check that all Letter Buttons are initialized as neutral. 
            Debug.Assert(LetterButtonsNeutral(i),
                         $"Letter Buttons for Controller {i} should be neutral.")

            ' Check that additional Right Thumbstick states are not active.
            Debug.Assert(Not RightThumbstickLeft(i),
                         $"Right Thumbstick Left for Controller {i} should not be true.")
            Debug.Assert(Not RightThumbstickRight(i),
                         $"Right Thumbstick Right for Controller {i} should not be true.")
            Debug.Assert(Not RightThumbstickDown(i),
                         $"Right Thumbstick Down for Controller {i} should not be true.")
            Debug.Assert(Not RightThumbstickUp(i),
                         $"Right Thumbstick Up for Controller {i} should not be true.")

            ' Check that additional Left Thumbstick states are not active.
            Debug.Assert(Not LeftThumbstickLeft(i),
                         $"Left Thumbstick Left for Controller {i} should not be true.")
            Debug.Assert(Not LeftThumbstickRight(i),
                         $"Left Thumbstick Right for Controller {i} should not be true.")
            Debug.Assert(Not LeftThumbstickDown(i),
                         $"Left Thumbstick Down for Controller {i} should not be true.")
            Debug.Assert(Not LeftThumbstickUp(i),
                         $"Left Thumbstick Up for Controller {i} should not be true.")

            ' Check that trigger states are not active.
            Debug.Assert(Not LeftTrigger(i),
                         $"Left Trigger for Controller {i} should not be true.")
            Debug.Assert(Not RightTrigger(i),
                         $"Right Trigger for Controller {i} should not be true.")

            ' Check that letter button states (A, B, X, Y) are not active.
            Debug.Assert(Not A(i),
                         $"A for Controller {i} should not be true.")
            Debug.Assert(Not B(i),
                         $"B for Controller {i} should not be true.")
            Debug.Assert(Not X(i),
                         $"X for Controller {i} should not be true.")
            Debug.Assert(Not Y(i),
                         $"Y for Controller {i} should not be true.")

            ' Check that bumper button states are not active.
            Debug.Assert(Not LeftBumper(i),
                         $"Left Bumper for Controller {i} should not be true.")
            Debug.Assert(Not RightBumper(i),
                         $"Right Bumper for Controller {i} should not be true.")

            ' Check that D-Pad directional states are not active.
            Debug.Assert(Not DPadUp(i),
                         $"D-Pad Up for Controller {i} should not be true.")
            Debug.Assert(Not DPadDown(i),
                         $"D-Pad Down for Controller {i} should not be true.")
            Debug.Assert(Not DPadLeft(i),
                         $"D-Pad Left for Controller {i} should not be true.")
            Debug.Assert(Not DPadRight(i),
                         $"D-Pad Right for Controller {i} should not be true.")

            ' Check that start and back button states are not active.
            Debug.Assert(Not Start(i),
                         $"Start Button for Controller {i} should not be true.")
            Debug.Assert(Not Back(i),
                         $"Back Button for Controller {i} should not be true.")

            ' Check that stick button states are not active.
            Debug.Assert(Not LeftStick(i),
                         $"Left Stick for Controller {i} should not be true.")
            Debug.Assert(Not RightStick(i),
                         $"Right Stick for Controller {i} should not be true.")

            Debug.Assert(Not LeftVibrateStart(i) = Nothing,
                         $"Left Vibrate Start for Controller {i} should not be Nothing.")
            Debug.Assert(Not RightVibrateStart(i) = Nothing,
                         $"Right Vibrate Start for Controller {i} should not be Nothing.")

            Debug.Assert(Not IsLeftVibrating(i),
                         $"Is Left Vibrating for Controller {i} should not be true.")
            Debug.Assert(Not IsRightVibrating(i),
                         $"Is Right Vibrating for Controller {i} should not be true.")

        Next

        ' For Lex

    End Sub

    <DllImport("XInput1_4.dll")>
    Private Shared Function XInputSetState(playerIndex As Integer,
                                     ByRef vibration As XINPUT_VIBRATION) As Integer
    End Function

    Private Structure XINPUT_VIBRATION

        Public wLeftMotorSpeed As UShort
        Public wRightMotorSpeed As UShort
    End Structure

    Private Vibration As XINPUT_VIBRATION

    Public Sub VibrateLeft(CID As Integer, Speed As UShort)
        ' The range of speed is 0 through 65,535. Unsigned 16-bit (2-byte) integer.
        ' The left motor is the low-frequency rumble motor.

        ' Set left motor speed.
        Vibration.wLeftMotorSpeed = Speed

        LeftVibrateStart(CID) = Now

        IsLeftVibrating(CID) = True

    End Sub

    Public Sub VibrateRight(CID As Integer, Speed As UShort)
        ' The range of speed is 0 through 65,535. Unsigned 16-bit (2-byte) integer.
        ' The right motor is the high-frequency rumble motor.

        ' Set right motor speed.
        Vibration.wRightMotorSpeed = Speed

        RightVibrateStart(CID) = Now

        IsRightVibrating(CID) = True

    End Sub

    Private Sub SendVibrationMotorCommand(ControllerID As Integer)
        ' Sends vibration motor speed command to the specified controller.

        Try

            ' Send motor speed command to the specified controller.
            If XInputSetState(ControllerID, Vibration) = 0 Then
                ' The motor speed was set. Success.

            Else
                ' The motor speed was not set. Fail.

                Debug.Print($"{ControllerID} did not vibrate.  {Vibration.wLeftMotorSpeed} |  {Vibration.wRightMotorSpeed} ")

            End If

        Catch ex As Exception

            Debug.Print($"Error sending vibration motor command: {ControllerID} | {Vibration.wLeftMotorSpeed} |  {Vibration.wRightMotorSpeed} | {ex.Message}")

            Exit Sub

        End Try

    End Sub

    Private Sub UpdateVibrateTimers()

        UpdateLeftVibrateTimer()

        UpdateRightVibrateTimer()

    End Sub

    Private Sub UpdateLeftVibrateTimer()

        For ControllerNumber As Integer = 0 To 3

            If IsLeftVibrating(ControllerNumber) Then

                Dim ElapsedTime As TimeSpan = Now - LeftVibrateStart(ControllerNumber)

                If ElapsedTime.TotalMilliseconds >= TimeToVibe Then

                    IsLeftVibrating(ControllerNumber) = False

                    ' Turn left motor off (set zero speed).
                    Vibration.wLeftMotorSpeed = 0

                End If

                SendVibrationMotorCommand(ControllerNumber)

            End If

        Next

    End Sub

    Private Sub UpdateRightVibrateTimer()

        For ControllerNumber As Integer = 0 To 3

            If IsRightVibrating(ControllerNumber) Then

                Dim ElapsedTime As TimeSpan = Now - RightVibrateStart(ControllerNumber)

                If ElapsedTime.TotalMilliseconds >= TimeToVibe Then

                    IsRightVibrating(ControllerNumber) = False

                    ' Turn left motor off (set zero speed).
                    Vibration.wRightMotorSpeed = 0

                End If

                SendVibrationMotorCommand(ControllerNumber)

            End If

        Next

    End Sub

End Structure




Public Structure BufferManager

    Private Context As BufferedGraphicsContext

    Public Buffered As BufferedGraphics

    Private ReadOnly MinimumBufferSize As Size '(1280, 720)

    Private BackgroundColor As Color '= Color.Black

    Public Sub New(form As Form, backgroundColor As Color)

        Me.BackgroundColor = backgroundColor

        MinimumBufferSize = New Size(1280, 720)

        InitializeBuffer(form)

    End Sub

    Private Sub InitializeBuffer(form As Form)

        Context = BufferedGraphicsManager.Current

        If Screen.PrimaryScreen IsNot Nothing Then

            Context.MaximumBuffer = Screen.PrimaryScreen.WorkingArea.Size

        Else

            Context.MaximumBuffer = MinimumBufferSize

            Debug.Print($"Primary screen not detected.")

        End If

        AllocateBuffer(form)

    End Sub

    Public Sub AllocateBuffer(form As Form)

        If Buffered Is Nothing Then

            Buffered = Context.Allocate(form.CreateGraphics(), form.ClientRectangle)

            Buffered.Graphics.CompositingMode =
                              CompositingMode.SourceOver

            Buffered.Graphics.CompositingQuality =
                              CompositingQuality.HighQuality

            Buffered.Graphics.SmoothingMode =
                              SmoothingMode.HighQuality

            Buffered.Graphics.InterpolationMode =
                              InterpolationMode.Bicubic

            Buffered.Graphics.PixelOffsetMode =
                              PixelOffsetMode.HighQuality

            Buffered.Graphics.TextRenderingHint =
                         Text.TextRenderingHint.AntiAliasGridFit

            EraseFrame()

        End If

    End Sub

    Public Sub EraseFrame()

        Buffered?.Graphics.Clear(BackgroundColor)

    End Sub

    Public Sub DisposeBuffer()

        If Buffered IsNot Nothing Then

            Buffered.Dispose()

            Buffered = Nothing ' Set to Nothing to avoid using a disposed object

            ' The buffer will be reallocated in OnPaint

        End If

    End Sub

End Structure

Public Class Form1

    Private DeltaTime As New DeltaTimeStructure(Now, Now, TimeSpan.Zero)

    Private OffScreen As New BufferManager(Me, BackColor)

    Private Player As AudioPlayer

    Private Turret As Turret

    Private Projectiles As New ProjectileManager(Brushes.Red, New Drawing.Size(10, 10), 600, 100, 9)

    Private Controllers As XboxControllers

    Private ClientCenter As Point = New Point(ClientSize.Width / 2, ClientSize.Height / 2)

    Private Body As New Body(Brushes.Gray, New PointF(500, 500), 128, 64, 0, 100, 100, 400, 100)

    Dim Arrow As New ArrowVector(New Pen(Color.Black, 10), New PointF(640, 360), 0, 60, 70, 10, 15, 0, Body.MaxVelocity, 30)

    Private RelativeTurretAngle As Single

    Private ADown As Boolean

    Private DDown As Boolean

    Private WDown As Boolean

    Private SDown As Boolean

    Private EDown As Boolean

    Private F1Down As Boolean

    Private F1DownHandled As Boolean

    Private F2Down As Boolean

    Private F2DownHandled As Boolean

    Private XDown As Boolean

    Private LeftArrowDown As Boolean

    Private RightArrowDown As Boolean

    Private SpaceDown As Boolean

    Private TimeToNextRotation As TimeSpan = TimeSpan.FromMilliseconds(1)

    Private LastRotationTime As DateTime = Now

    Private InstructionsFont As New Font("Segoe UI", 14)

    Private InstructionsLocation As New Point(0, 0)

    Private F1NoticeLocation As New PointF(0, 0)

    Private HiddenHints As New String("Show / Hide Keyboard Hints > F1" _
                                  & Environment.NewLine)

    Private KeyboardHintsText As New String("Show / Hide Keyboard Hints > F1" _
                                  & Environment.NewLine _
                                  & "Rotate Turret CounterClockwise > Left Arrow" _
                                  & Environment.NewLine _
                                  & "Rotate Turret Clockwise > Right Arrow" _
                                  & Environment.NewLine _
                                  & "Show / Hide Controller Hints > F2" _
                                  & Environment.NewLine _
                                  & "Rotate Hull Counterclockwise > A" _
                                  & Environment.NewLine _
                                  & "Rotate Hull Clockwise > D" _
                                  & Environment.NewLine _
                                  & "Forward > W" _
                                  & Environment.NewLine _
                                  & "Reverse > S" _
                                  & Environment.NewLine _
                                  & "Stop > E" _
                                  & Environment.NewLine _
                                  & "Fire > Space" _
                                  & Environment.NewLine)


    Private ControllerHintsText As New String("Show / Hide Keyboard Hints > F1" _
                                  & Environment.NewLine _
                                  & "Rotate Turret CounterClockwise > Right Thumbstick Left" _
                                  & Environment.NewLine _
                                  & "Rotate Turret Clockwise > Right Thumbstick Right" _
                                  & Environment.NewLine _
                                  & "Rotate Hull Counterclockwise > Left Thumbstick Left" _
                                  & Environment.NewLine _
                                  & "Rotate Hull Clockwise > Left Thumbstick Right" _
                                  & Environment.NewLine _
                                  & "Show / Hide Controller Hints > F2" _
                                  & Environment.NewLine _
                                  & "Forward > A" _
                                  & Environment.NewLine _
                                  & "Reverse > Y" _
                                  & Environment.NewLine _
                                  & "Stop > B" _
                                  & Environment.NewLine _
                                  & "Fire > Right Trigger" _
                                  & Environment.NewLine)

    Private HintsText As String = KeyboardHintsText

    Public Sub New()
        InitializeComponent()

        InitializeForm()

        InitializeTimer()

        Controllers.Initialize()

        InitializeSounds()

        Body.TimeToNextRotation = TimeSpan.FromMilliseconds(25)

        Turret = New Turret(New Pen(Color.Black, 10), 50, Body.Center, 75, 0, TimeSpan.FromMilliseconds(100))

        Projectiles.BarrelLength = Turret.Length

        RelativeTurretAngle = Turret.AngleInDegrees - Body.AngleInDegrees

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick

        DeltaTime.Update()

        HandleUserInput()

        Projectiles.UpdateProjectiles(DeltaTime.ElapsedTime)

        Body.CheckWraparound(Body.Body, ClientSize.Width, ClientSize.Height)

        Body.Update(DeltaTime.ElapsedTime)

        Turret.AngleInDegrees = (Body.AngleInDegrees + RelativeTurretAngle + 360) Mod 360

        Arrow.Center = Body.Center

        Arrow.AngleInDegrees = Body.AngleInDegrees

        Arrow.Velocity = Body.Velocity

        Arrow.Update(DeltaTime.ElapsedTime)

        Turret.Center = Body.Center

        HandleAudioPlayback()

        Invalidate()

    End Sub

    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)

        DrawFrame()

        ' Show buffer on form.
        OffScreen.Buffered?.Render(e.Graphics)

        OffScreen.EraseFrame()

    End Sub

    Protected Overrides Sub OnKeyDown(e As KeyEventArgs)
        MyBase.OnKeyDown(e)

        Select Case e.KeyCode
            Case Keys.A
                ADown = True
            Case Keys.D
                DDown = True
            Case Keys.W
                WDown = True
            Case Keys.S
                SDown = True
            Case Keys.E
                EDown = True
            Case Keys.F1
                F1Down = True
            Case Keys.F2
                F2Down = True
            Case Keys.X
                XDown = True
            Case Keys.Left
                LeftArrowDown = True
            Case Keys.Right
                RightArrowDown = True
            Case Keys.Space
                SpaceDown = True

        End Select

    End Sub

    Protected Overrides Sub OnKeyUp(e As KeyEventArgs)
        MyBase.OnKeyUp(e)

        Select Case e.KeyCode
            Case Keys.A
                ADown = False
            Case Keys.D
                DDown = False
            Case Keys.W
                WDown = False
            Case Keys.S
                SDown = False
            Case Keys.E
                EDown = False
            Case Keys.F1
                F1Down = False
                F1DownHandled = False
            Case Keys.F2
                F2Down = False
                F2DownHandled = False
            Case Keys.X
                XDown = False
            Case Keys.Left
                LeftArrowDown = False
            Case Keys.Right
                RightArrowDown = False
            Case Keys.Space
                SpaceDown = False

        End Select

    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize

        If Not WindowState = FormWindowState.Minimized Then

            ClientCenter = New Point(ClientSize.Width / 2, ClientSize.Height / 2)

            Body.ScaleFactor = ClientSize.Height / 40

            Body.Center = ClientCenter

            OffScreen.DisposeBuffer()

            Timer1.Enabled = True

        Else

            Timer1.Enabled = False

        End If

    End Sub

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles Me.Load

        ClientCenter = New Point(ClientSize.Width / 2, ClientSize.Height / 2)

    End Sub

    Private Sub DrawFrame()

        OffScreen.AllocateBuffer(Me)

        OffScreen.Buffered.Graphics.DrawString(HintsText, InstructionsFont, Brushes.Black, InstructionsLocation)

        Body.Draw(OffScreen.Buffered.Graphics)

        'Arrow.Draw(OffScreen.Buffered.Graphics)

        Projectiles.DrawProjectiles(OffScreen.Buffered.Graphics)

        'Turret.Draw(OffScreen.Buffered.Graphics)

    End Sub

    Private Sub HandleUserInput()

        Controllers.Update()

        HandleKeyPresses()

        HandleControllerInput()

    End Sub

    Private Sub HandleKeyPresses()
        ' Handle key presses to rotate the turret or fire projectiles.

        If ADown Then

            Body.RotateCounterClockwise()

        End If

        If DDown Then

            Body.RotateClockwise()

        End If

        If WDown Then

            If Body.Velocity < Body.MaxVelocity Then

                Body.Velocity += Body.Acceleration.X * DeltaTime.ElapsedTime.TotalSeconds

            Else

                Body.Velocity = Body.MaxVelocity

            End If

        ElseIf SDown Then

            If Body.Velocity > -Body.MaxVelocity Then

                Body.Velocity += -1

            Else

                Body.Velocity = -Body.MaxVelocity

            End If

        ElseIf Not Controllers.A(0) Then

            Body.Decelerate(DeltaTime.ElapsedTime)

        End If

        'If EDown Then

        '    Body.EmergencyStop(DeltaTime.ElapsedTime)

        '    If Body.Velocity <> 0 Then

        '        If Not Player.IsPlaying("emergencystop") Then

        '            Player.PlaySound("emergencystop")

        '        End If

        '    Else

        '        If Player.IsPlaying("emergencystop") Then

        '            Player.PauseSound("emergencystop")

        '        End If

        '    End If

        'ElseIf Not Controllers.B(0) Then

        '    If Player.IsPlaying("emergencystop") Then

        '        Player.PauseSound("emergencystop")

        '    End If

        'End If

        If F1Down Then

            If Body.ShowControllerHints Then Body.ShowControllerHints = False

            If Body.ShowKeyboardHints Then

                If Not F1DownHandled Then

                    Body.ShowKeyboardHints = False

                    HintsText = HiddenHints

                    F1DownHandled = True

                End If

            Else

                If Not F1DownHandled Then

                    Body.ShowKeyboardHints = True

                    HintsText = KeyboardHintsText

                    F1DownHandled = True

                End If

            End If

        End If

        If F2Down Then

            If Body.ShowKeyboardHints Then Body.ShowKeyboardHints = False

            If Body.ShowControllerHints Then

                If Not F2DownHandled Then

                    Body.ShowControllerHints = False

                    HintsText = HiddenHints

                    F2DownHandled = True

                End If

            Else

                If Not F2DownHandled Then

                    Body.ShowControllerHints = True

                    HintsText = ControllerHintsText

                    F2DownHandled = True

                End If

            End If

        End If

        If XDown OrElse SpaceDown Then

            FireProjectile()

        End If

        If LeftArrowDown Then

            'RotateTurretCounterClockwise()

        End If

        If RightArrowDown Then

            'RotateTurretClockwise()

        End If

    End Sub

    Private Sub RotateTurretClockwise()

        Dim TimeSinceLastRotation As TimeSpan = Now - LastRotationTime

        If TimeSinceLastRotation > TimeToNextRotation Then

            ' Normalize and increment the turret angle
            Dim AngleInDegrees = (Turret.AngleInDegrees + 1) Mod 360

            RelativeTurretAngle = (AngleInDegrees - Body.AngleInDegrees + 360) Mod 360

            LastRotationTime = Now

        End If

    End Sub

    Private Sub RotateTurretCounterClockwise()

        Dim TimeSinceLastRotation As TimeSpan = Now - LastRotationTime

        If TimeSinceLastRotation > TimeToNextRotation Then

            ' Normalize and decrement the turret angle
            Dim AngleInDegrees = (Turret.AngleInDegrees - 1 + 360) Mod 360

            RelativeTurretAngle = (AngleInDegrees - Body.AngleInDegrees + 360) Mod 360

            LastRotationTime = Now

        End If

    End Sub

    Private Sub FireProjectile()

        ' Is it time to shoot my shot?
        If Now - Turret.LastFireTime > Turret.TimeToNextFire Then
            ' Yes, it's time to shoot your shot.

            Player.PlayOverlapping("gunshot")

            Projectiles.FireProjectile(Turret.Center, Turret.AngleInDegrees)

            Turret.LastFireTime = Now

        End If

    End Sub

    Private Sub HandleControllerInput()

        If Controllers.LeftThumbstickLeft(0) Then

            Body.RotateCounterClockwise()

        End If

        If Controllers.LeftThumbstickRight(0) Then

            Body.RotateClockwise()

        End If

        If Controllers.A(0) OrElse Controllers.LeftStick(0) Then

            If Body.Velocity < Body.MaxVelocity Then

                Body.Velocity += Body.Acceleration.X * DeltaTime.ElapsedTime.TotalSeconds

            Else

                Body.Velocity = Body.MaxVelocity

            End If

        ElseIf Controllers.Y(0) Then

            If Body.Velocity > -Body.MaxVelocity Then

                Body.Velocity += -1

            Else

                Body.Velocity = -Body.MaxVelocity

            End If

        ElseIf Not WDown Then

            Body.Decelerate(DeltaTime.ElapsedTime)

        End If

        'If Controllers.B(0) Then

        '    Body.EmergencyStop(DeltaTime.ElapsedTime)

        '    If Body.Velocity <> 0 Then

        '        If Not Player.IsPlaying("emergencystop") Then

        '            Player.PlaySound("emergencystop")

        '        End If

        '        Controllers.TimeToVibe = 50

        '        Controllers.VibrateRight(0, 32000)

        '    Else

        '        If Player.IsPlaying("emergencystop") Then

        '            Player.PauseSound("emergencystop")

        '        End If

        '    End If

        'ElseIf Not EDown Then

        '    If Player.IsPlaying("emergencystop") Then

        '        Player.PauseSound("emergencystop")

        '    End If

        'End If

        If Controllers.X(0) OrElse Controllers.RightTrigger(0) Then

            FireProjectile()

        End If

        If Controllers.RightThumbstickLeft(0) Then

            'RotateTurretCounterClockwise()

        End If

        If Controllers.RightThumbstickRight(0) Then

            'RotateTurretClockwise()

        End If

    End Sub

    Private Sub HandleAudioPlayback()

        If Body.Velocity <> Body.MinVelocity Then

            If Not Player.IsPlaying("airplanemax") Then

                Player.LoopSound("airplanemax")

            End If

            If Player.IsPlaying("airplanemin") Then

                Player.PauseSound("airplanemin")

            End If

        Else

            If Not Player.IsPlaying("airplanemin") Then

                Player.LoopSound("airplanemin")

            End If

            If Player.IsPlaying("airplanemax") Then

                Player.PauseSound("airplanemax")

            End If

        End If

    End Sub

    Private Sub InitializeForm()

        CenterToScreen()

        SetStyle(ControlStyles.UserPaint, True)

        ' Enable double buffering to reduce flickering
        SetStyle(ControlStyles.OptimizedDoubleBuffer Or ControlStyles.AllPaintingInWmPaint, True)

        Text = "Airplane - Code with Joe"

        WindowState = FormWindowState.Maximized

    End Sub

    Private Sub InitializeTimer()

        Timer1.Interval = 15

        Timer1.Start()

    End Sub

    Private Sub InitializeSounds()

        CreateSoundFiles()

        Dim FilePath As String = Path.Combine(Application.StartupPath, "idle.mp3")

        Player.AddSound("idle", FilePath)

        Player.SetVolume("idle", 300)

        FilePath = Path.Combine(Application.StartupPath, "running.mp3")

        Player.AddSound("running", FilePath)

        Player.SetVolume("running", 400)

        FilePath = Path.Combine(Application.StartupPath, "emergencystop.mp3")

        Player.AddSound("emergencystop", FilePath)

        Player.SetVolume("emergencystop", 1000)

        FilePath = Path.Combine(Application.StartupPath, "gunshot.mp3")

        Player.AddOverlapping("gunshot", FilePath)

        Player.SetVolumeOverlapping("gunshot", 500)

        FilePath = Path.Combine(Application.StartupPath, "airplanemin.mp3")

        Player.AddSound("airplanemin", FilePath)

        Player.SetVolume("airplanemin", 1000)

        FilePath = Path.Combine(Application.StartupPath, "airplanemax.mp3")

        Player.AddSound("airplanemax", FilePath)

        Player.SetVolume("airplanemax", 1000)

        Player.LoopSound("airplanemin")

    End Sub

    Private Sub CreateSoundFiles()

        Dim FilePath As String

        FilePath = Path.Combine(Application.StartupPath, "emergencystop.mp3")
        CreateFileFromResource(FilePath, My.Resources.Resource1.emergencystop)

        FilePath = Path.Combine(Application.StartupPath, "gunshot.mp3")
        CreateFileFromResource(FilePath, My.Resources.Resource1.gunshot003)

        FilePath = Path.Combine(Application.StartupPath, "airplanemin.mp3")
        CreateFileFromResource(FilePath, My.Resources.Resource1.airplanemin)

        FilePath = Path.Combine(Application.StartupPath, "airplanemax.mp3")
        CreateFileFromResource(FilePath, My.Resources.Resource1.airplanemax)

    End Sub

    Private Sub CreateFileFromResource(filepath As String, resource As Byte())

        Try

            If Not IO.File.Exists(filepath) Then

                IO.File.WriteAllBytes(filepath, resource)

            End If

        Catch ex As Exception

            Debug.Print($"Error creating file: {ex.Message}")

        End Try

    End Sub

End Class



