//
// Fingers Lite Gestures
// (c) 2015 Digital Ruby, LLC
// http://www.digitalruby.com
// Source code may be used for personal or commercial projects.
// Source code may NOT be redistributed or sold.
// Please see license.txt file
// 


using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DigitalRubyShared
{
    public class MainGestures : MonoBehaviour
    {
        public GameObject mainObject;
        public Material LineMaterial;


        private TapGestureRecognizer tapGesture;
        private TapGestureRecognizer doubleTapGesture;
        private TapGestureRecognizer tripleTapGesture;
        private SwipeGestureRecognizer swipeGesture;
        private PanGestureRecognizer panGesture;
        private ScaleGestureRecognizer scaleGesture;
        private RotateGestureRecognizer rotateGesture;
        private LongPressGestureRecognizer longPressGesture;

        private readonly List<Vector3> swipeLines = new List<Vector3>();

        private void DebugText(string text, params object[] format)
        {
            //bottomLabel.text = string.Format(text, format);
            Debug.Log(string.Format(text, format));
        }


        private void HandleSwipe(float endX, float endY)
        {
            Vector2 start = new Vector2(swipeGesture.StartFocusX, swipeGesture.StartFocusY);
            Vector3 startWorld = Camera.main.ScreenToWorldPoint(start);
            Vector3 endWorld = Camera.main.ScreenToWorldPoint(new Vector2(endX, endY));
            float distance = Vector3.Distance(startWorld, endWorld);
            startWorld.z = endWorld.z = 0.0f;

            swipeLines.Add(startWorld);
            swipeLines.Add(endWorld);

            if (swipeLines.Count > 4)
            {
                swipeLines.RemoveRange(0, swipeLines.Count - 4);
            }

            RaycastHit2D[] collisions = Physics2D.CircleCastAll(startWorld, 10.0f, (endWorld - startWorld).normalized, distance);

            if (collisions.Length != 0)
            {
                Debug.Log("Raycast hits: " + collisions.Length + ", start: " + startWorld + ", end: " + endWorld + ", distance: " + distance);

                Vector3 origin = Camera.main.ScreenToWorldPoint(Vector3.zero);
                Vector3 end = Camera.main.ScreenToWorldPoint(new Vector3(swipeGesture.VelocityX, swipeGesture.VelocityY, Camera.main.nearClipPlane));
                Vector3 velocity = (end - origin);
                Vector2 force = velocity * 500.0f;

                foreach (RaycastHit2D h in collisions)
                {
                    h.rigidbody.AddForceAtPosition(force, h.point);
                }
            }
        }

        private void SwipeGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Ended)
            {
                HandleSwipe(gesture.FocusX, gesture.FocusY);
                DebugText("Swiped from {0},{1} to {2},{3}; velocity: {4}, {5}", gesture.StartFocusX, gesture.StartFocusY, gesture.FocusX, gesture.FocusY, swipeGesture.VelocityX, swipeGesture.VelocityY);
            }
        }

        private void CreateSwipeGesture()
        {
            swipeGesture = new SwipeGestureRecognizer();
            swipeGesture.Direction = SwipeGestureRecognizerDirection.Any;
            swipeGesture.StateUpdated += SwipeGestureCallback;
            swipeGesture.DirectionThreshold = 1.0f; // allow a swipe, regardless of slope
            FingersScript.Instance.AddGesture(swipeGesture);
        }

        private void PanGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Executing)
            {
                DebugText("Panned, Location: {0}, {1}, Delta: {2}, {3}", gesture.FocusX, gesture.FocusY, gesture.DeltaX, gesture.DeltaY);

                float deltaX = panGesture.DeltaX / 25.0f;
                float deltaY = panGesture.DeltaY / 25.0f;
                Vector3 pos = mainObject.transform.position;
                pos.x += deltaX;
                pos.y += deltaY;
                mainObject.transform.position = pos;
            }
        }

        private void CreatePanGesture()
        {
            panGesture = new PanGestureRecognizer();
            panGesture.MinimumNumberOfTouchesToTrack = 2;
            panGesture.StateUpdated += PanGestureCallback;
            FingersScript.Instance.AddGesture(panGesture);
        }

        private void ScaleGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Executing)
            {
                DebugText("Scaled: {0}, Focus: {1}, {2}", scaleGesture.ScaleMultiplier, scaleGesture.FocusX, scaleGesture.FocusY);
                mainObject.transform.localScale *= scaleGesture.ScaleMultiplier;
            }
        }

        private void CreateScaleGesture()
        {
            scaleGesture = new ScaleGestureRecognizer();
            scaleGesture.StateUpdated += ScaleGestureCallback;
            FingersScript.Instance.AddGesture(scaleGesture);
        }

        private void RotateGestureCallback(GestureRecognizer gesture)
        {
            if (gesture.State == GestureRecognizerState.Executing)
            {
                mainObject.transform.Rotate(0.0f, 0.0f, rotateGesture.RotationRadiansDelta * Mathf.Rad2Deg);
            }
        }

        private void CreateRotateGesture()
        {
            rotateGesture = new RotateGestureRecognizer();
            rotateGesture.StateUpdated += RotateGestureCallback;
            FingersScript.Instance.AddGesture(rotateGesture);
        }

        private static bool? CaptureGestureHandler(GameObject obj)
        {
            // I've named objects PassThrough* if the gesture should pass through and NoPass* if the gesture should be gobbled up, everything else gets default behavior
            if (obj.name.StartsWith("PassThrough"))
            {
                // allow the pass through for any element named "PassThrough*"
                return false;
            }
            else if (obj.name.StartsWith("NoPass"))
            {
                // prevent the gesture from passing through, this is done on some of the buttons and the bottom text so that only
                // the triple tap gesture can tap on it
                return true;
            }

            // fall-back to default behavior for anything else
            return null;
        }

        private void Start()
        {

            // don't reorder the creation of these :)
            CreateSwipeGesture();
            CreatePanGesture();
            CreateScaleGesture();
            CreateRotateGesture();

            // pan, scale and rotate can all happen simultaneously
            panGesture.AllowSimultaneousExecution(scaleGesture);
            panGesture.AllowSimultaneousExecution(rotateGesture);
            scaleGesture.AllowSimultaneousExecution(rotateGesture);

            // prevent the one special no-pass button from passing through,
            //  even though the parent scroll view allows pass through (see FingerScript.PassThroughObjects)
            FingersScript.Instance.CaptureGestureHandler = CaptureGestureHandler;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ReloadDemoScene();
            }
        }

        private void LateUpdate()
        {

            int touchCount = Input.touchCount;
            if (FingersScript.Instance.TreatMousePointerAsFinger && Input.mousePresent)
            {
                touchCount += (Input.GetMouseButton(0) ? 1 : 0);
                touchCount += (Input.GetMouseButton(1) ? 1 : 0);
                touchCount += (Input.GetMouseButton(2) ? 1 : 0);
            }
            string touchIds = string.Empty;
            int gestureTouchCount = 0;
            foreach (GestureRecognizer g in FingersScript.Instance.Gestures)
            {
                gestureTouchCount += g.CurrentTrackedTouches.Count;
            }
            foreach (GestureTouch t in FingersScript.Instance.CurrentTouches)
            {
                touchIds += ":" + t.Id + ":";
            }

            //dpiLabel.text = "Dpi: " + DeviceInfo.PixelsPerInch + System.Environment.NewLine +
            //    "Width: " + Screen.width + System.Environment.NewLine +
            //    "Height: " + Screen.height + System.Environment.NewLine +
            //    "Touches: " + FingersScript.Instance.CurrentTouches.Count + " (" + gestureTouchCount + "), ids" + touchIds + System.Environment.NewLine;
        }

        private void OnRenderObject()
        {
            if (LineMaterial == null)
            {
                return;
            }

            GL.PushMatrix();
            LineMaterial.SetPass(0);
            GL.LoadProjectionMatrix(Camera.main.projectionMatrix);
            GL.Begin(GL.LINES);
            for (int i = 0; i < swipeLines.Count; i++)
            {
                GL.Color(Color.yellow);
                GL.Vertex(swipeLines[i]);
                GL.Vertex(swipeLines[++i]);
            }
            GL.End();
            GL.PopMatrix();
        }

        public void ReloadDemoScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0, UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }
}
