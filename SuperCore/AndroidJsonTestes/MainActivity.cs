using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Android.App;
using Android.Widget;
using Android.OS;
using SuperJson;
using SuperJson.Parser;

namespace AndroidJsonTestes
{
    public class TestClass
    {
        private static readonly Lazy<TestClass> mInstance = new Lazy<TestClass>(() => new TestClass());
        public static TestClass Instance => mInstance.Value;

        [DoNotSerialise]
        const string FileName = "Data.json";
        [DoNotSerialise]
        private List<TestClass> mProjectsCashe;
        [DoNotSerialise]
        private readonly ConcurrentDictionary<int, List<TestClass>> mVolumeCashe = new ConcurrentDictionary<int, List<TestClass>>();
        [DoNotSerialise]
        private readonly ConcurrentDictionary<int, List<TestClass>> mChaptersCache = new ConcurrentDictionary<int, List<TestClass>>();
        [DoNotSerialise]
        private readonly ConcurrentDictionary<int, TestClass> mTextCace = new ConcurrentDictionary<int, TestClass>();
        [DoNotSerialise]
        private int mLastChapterId;
        [DoNotSerialise]

        public bool Loaded { get; private set; }
        public List<string> ReadChapterUrls { get; private set; } = new List<string>();
        public Dictionary<string, int> ChapterSctolls { get; private set; } = new Dictionary<string, int>();
        //[DoNotSerialise]
        public Dictionary<int, int> ProjectOrders { get; private set; } = new Dictionary<int, int>();
    }

    [Activity(Label = "AndroidJsonTestes", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView (Resource.Layout.Main);

            var obj = new TestClass();
            for (int i = 0; i < 150; i++)
            {
                obj.ProjectOrders[i] = 0;
                obj.ChapterSctolls[i.ToString()] = i % 4;
                obj.ReadChapterUrls.Add(i.ToString());
            }

            var ser = new SuperJsonSerializer();
            var data = ser.Serialize(obj);
        }
    }
}

