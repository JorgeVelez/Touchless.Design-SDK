﻿using System;

namespace TouchlessDesign.Components {

  public abstract class AppComponent {

    public static AppComponent[] Components { get; set; }
    
    public static Ui.Ui Ui { get; set; }
    
    public static Input.Input Input { get; set; }
    
    public static Ipc.Ipc Ipc { get; set; }

    public static Lighting.Lighting Lighting { get; set; }

    public event Action OnStarted;
    public event Action OnStopped;

    public bool IsStarted { get; private set; }
    public string DataDir { get; private set; }


    public void Start() {
      if (IsStarted) return;
      IsStarted = true;
      DoStart();
      OnStarted?.Invoke();
    }

    public void Stop() {
      if (!IsStarted) return;
      IsStarted = false;
      DoStop();
      OnStopped?.Invoke();
    }

    protected abstract void DoStart();

    protected abstract void DoStop();


    public static void InitializeComponents(App app) {
      Ui = new Ui.Ui(app);
      Input = new Input.Input();
      Lighting = new Lighting.Lighting();
      Ipc = new Ipc.Ipc();
      Components = new AppComponent[] {
        Ui,
        Input,
        Lighting,
        Ipc
      };
      foreach (var c in Components) {
        try {
          c.DataDir = app.DataDir;
          c.Start();
        }
        catch (Exception e) {
          Log.Error($"Error starting AppComponent of type {c.GetType().Name}: {e}");
        }
      }
    }


    public static void DeInitializeComponents() {
      foreach (var c in Components) {
        try {
          c.Stop();
        }
        catch (Exception e) {
          Log.Error(e);
        }
      }
      Components = null;
      Ui = null;
      Input = null;
      Lighting = null;
      Ipc = null;
    }
  }
}