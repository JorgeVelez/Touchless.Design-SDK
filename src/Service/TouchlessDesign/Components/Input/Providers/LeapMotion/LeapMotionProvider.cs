﻿using System.Collections.Generic;
using System.Threading;
using Leap;

namespace TouchlessDesign.Components.Input.Providers.LeapMotion {
  public class LeapMotionProvider : IInputProvider {

    private const float MillimetersToMeters = 0.001f;

    public string DataDir { get; set; }

    private LeapTransform _xform;
    private Controller _controller;

    private readonly List<Hand> _handsToRemoveBuffer = new List<Hand>();

    public void Start() {
      _xform = new LeapTransform(Vector.Zero, LeapQuaternion.Identity, new Vector(MillimetersToMeters, MillimetersToMeters, MillimetersToMeters));
      _xform.MirrorZ();
      _controller = new Controller();
      _controller.Connect += HandleLeapConnected;
      _controller.Disconnect += HandleLeapDisconnected;
      _controller.StartConnection();
    }

    public void Stop() {
      _controller.StopConnection();
      _controller.Connect -= HandleLeapConnected;
      _controller.Disconnect -= HandleLeapDisconnected;
      _controller = null;
    }

    private void HandleLeapConnected(object sender, ConnectionEventArgs e) {
      Log.Info("Leap Connected.");
    }

    private void HandleLeapDisconnected(object sender, ConnectionLostEventArgs e) {
      Log.Info("Leap Disconnected");
    }

    public bool Update(Dictionary<int, Hand> hands) {
      if (_controller == null || !_controller.IsConnected) {
        return false;
      }
      var f = _controller.Frame(0);
      _handsToRemoveBuffer.AddRange(hands.Values);
      foreach (var leapHand in f.Hands) {
        if (hands.TryGetValue(leapHand.Id, out var hand)) {
          hand.Apply(leapHand, _xform);
        }
        else {
          hand = new Hand(leapHand, _xform);
          hands.Add(leapHand.Id, hand);
        }
        _handsToRemoveBuffer.Remove(hand); //prevent this hand from being removed
      }

      foreach (var hand in _handsToRemoveBuffer) { //remove all hands that were not added or updated this frame
        hands.Remove(hand.Id);
      }
      _handsToRemoveBuffer.Clear();
      return true;
    }



    //private void HandleTimerTick(object state) {
    //  if (_controller == null || !_controller.IsConnected) return;
    //  if (!Cursor.IsEmulationEnabled) return;
    //  var f = _controller.Frame(0);

    //  if (f.Hands.Count <= 0) {
    //    if (Cursor.IsButtonDown) {
    //      Cursor.SetMouseButtonDown(false);
    //    }

    //    if (_hasClicked) {
    //      _hasClicked = false;
    //    }
    //  }
    //  else {
    //    var hand = f.Hands.First();

    //    //position
    //    var p = _xform.TransformPoint(hand.PalmPosition);
    //    _settings.NormalizedPosition(p, out var horizontal, out var vertical);
    //    var pixelX = Cursor.PixelX(horizontal);
    //    var pixelY = Cursor.PixelY(vertical);
    //    Cursor.SetPosition(pixelX, pixelY);

    //    //click
    //    var isGrabbing = hand.GrabStrength > _settings.GrabClickThreshold;
    //    Cursor.SetMouseDownConfidence(hand.GrabStrength);
    //    var hoverState = Cursor.GetHoverState();
    //    if (isGrabbing && !Cursor.IsButtonDown) {
    //      if (hoverState == HoverStates.Click) {
    //        if (!_hasClicked) {
    //          _hasClicked = true;
    //          Cursor.DoClick();
    //        }
    //        else {

    //        }
    //      }
    //      else {
    //        Cursor.SetMouseButtonDown(true);
    //      }
    //    }
    //    else if (!isGrabbing && Cursor.IsButtonDown) {
    //      Cursor.SetMouseButtonDown(false);
    //    }

    //    if (!isGrabbing && _hasClicked) {
    //      _hasClicked = false;
    //    }
    //  }
    //}
  }
}