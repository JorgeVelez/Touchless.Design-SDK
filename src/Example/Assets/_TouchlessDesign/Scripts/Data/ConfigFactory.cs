﻿using System;
using System.IO;
using Newtonsoft.Json;

namespace Ideum.Data {
  public static class ConfigFactory {
    public static T Get<T>(string path) where T : new() {
      return Get(path, () => new T());
    }

    public static T Get<T>(string path, Func<T> defaults) {
      try {
        if (File.Exists(path)) {
          var raw = File.ReadAllText(path);
          return JsonConvert.DeserializeObject<T>(raw);
        }
      }
      catch (Exception e) {
        Log.Error($"Error loading/deserializing file at {path}. {e}");
      }

      var d = defaults();
      Log.Warn($"Could not load {d.GetType().Name} from {path}. Creating and using defaults.");
      try {
        var s = JsonConvert.SerializeObject(d, Formatting.Indented);
        File.WriteAllText(path, s);
      }
      catch (Exception e) {
        Log.Error($"Error saving default settings: {e}");
      }

      return d;
    }

    public static void SaveConfig<T>(this T d, string path) {
      try {
        var s = JsonConvert.SerializeObject(d, Formatting.Indented);
        File.WriteAllText(path, s);
      }
      catch (Exception e) {
        Log.Error($"Error saving {typeof(T).Name} settings to {path}: {e}");
      }
    }
  }
}