using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace BrowserInterop.Performance
{
    /// <summary>
    /// provides access to performance-related information for the current page.
    /// </summary>
    public class PerformanceInterop
    {
        private readonly IJSRuntime jsRuntime;
        private JsRuntimeObjectRef windowRef;

        internal PerformanceInterop(IJSRuntime jsRuntime, JsRuntimeObjectRef windowRef)
        {
            this.jsRuntime = jsRuntime;
            this.windowRef = windowRef;
        }


        /// <summary>
        /// Represents the time at which the location was retrieved.
        /// </summary>
        /// <value></value>
        public async Task<double> TimeOrigin()
        {
            return await jsRuntime.GetInstancePropertyAsync<double>(windowRef, "performance.timeOrigin");
        }

        /// <summary>
        /// removes the named mark from the browser's performance entry buffer. If the method is called with no arguments, all performance entries with an entry type of "mark" will be removed from the performance entry buffer.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task ClearMarks(string name = null)
        {
            await jsRuntime.InvokeInstanceMethodAsync(windowRef, "performance.clearMarks", name);
        }

        /// <summary>
        /// emoves the named measure from the browser's performance entry buffer. If the method is called with no arguments, all performance entries with an entry type of "measure" will be removed from the performance entry buffer.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public async Task ClearMeasures(string name = null)
        {
            await jsRuntime.InvokeInstanceMethodAsync(windowRef, "performance.clearMeasures", name);
        }


        /// <summary>
        /// removes all performance entries with an entryType of "resource" from the browser's performance data buffer and sets the size of the performance data buffer to zero. To set the size of the browser's performance data buffer, use the Performance.setResourceTimingBufferSize() method.
        /// </summary>        
        /// <returns></returns>
        public async Task ClearResourceTimings()
        {
            await jsRuntime.InvokeInstanceMethodAsync(windowRef, "performance.clearResourceTimings");
        }

        /// <summary>
        /// returns a list of all PerformanceEntry objects for the page.
        /// </summary>
        /// <returns></returns>
        public async Task<PerformanceEntry[]> GetEntries()
        {
            return await jsRuntime.InvokeInstanceMethodAsync<PerformanceEntry[]>(windowRef, "performance.getEntries");
        }

        /// <summary>
        /// Returns a list of PerformanceEntry objects based on the given name
        /// </summary>
        /// <returns></returns>
        public async Task<PerformanceEntry[]> GetEntriesByName(string name)
        {
            return await jsRuntime.InvokeInstanceMethodAsync<PerformanceEntry[]>(windowRef, "performance.getEntriesByName", name);
        }

        /// <summary>
        /// Returns a list of PerformanceEntry objects based on the given name and entry type.
        /// </summary>
        /// <returns></returns>
        public async Task<T[]> GetEntriesByName<T>(string name) where T : PerformanceEntry
        {
            return await jsRuntime.InvokeInstanceMethodAsync<T[]>(windowRef, "performance.getEntriesByName", name, ConvertTypeToString(typeof(T)));
        }

        /// <summary>
        /// returns a list of all PerformanceEntry objects for the page.
        /// </summary>
        /// <returns></returns>
        public async Task<T[]> GetEntriesByType<T>() where T : PerformanceEntry
        {
            return await jsRuntime.InvokeInstanceMethodAsync<T[]>(windowRef, "performance.getEntriesByType", ConvertTypeToString(typeof(T)));
        }

        /// <summary>
        /// creates a timestamp in the browser's performance entry buffer with the given name. The application defined timestamp can be retrieved by one of the Performance interface's getEntries*() methods
        /// </summary>
        /// <param name="name">the name of the mark.</param>
        /// <returns></returns>
        public async Task Mark(string name)
        {
            await jsRuntime.InvokeInstanceMethodAsync(windowRef, "performance.mark", name);
        }

        /// <summary>
        /// The measure() method creates a named timestamp in the browser's performance entry buffer between marks, the navigation start time, or the current time. When measuring between two marks, there is a start mark and end mark, respectively. The named timestamp is referred to as a measure.
        /// </summary>
        /// <param name="name"> the name of the measure.</param>
        /// <param name="startMark"> the name of the measure's starting mark. May also be the name of a PerformanceTiming property. If it is omitted, then the start time will be the navigation start time.</param>
        /// <param name="endMark">the name of the measure's ending mark. May also be the name of a PerformanceTiming property. If it is omitted, then the current time is used.</param>
        /// <returns></returns>
        public async Task Measure(string name, string startMark = null, string endMark = null)
        {
            await jsRuntime.InvokeInstanceMethodAsync(windowRef, "performance.measure", name, startMark, endMark);
        }

        /// <summary>
        ///  returns a DOMHighResTimeStamp, measured in milliseconds. The returned value represents the time elapsed since the time origin
        /// </summary>
        /// <returns></returns>
        public async Task<double> Now()
        {
            return await jsRuntime.InvokeInstanceMethodAsync<double>(windowRef, "performance.now");
        }

        /// <summary>
        /// sets the browser's resource timing buffer size to the specified number of "resource" performance entry type objects.
        /// </summary>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        public async Task SetResourceTimingBufferSize(long maxSize)
        {
            await jsRuntime.InvokeInstanceMethodAsync(windowRef, "performance.setResourceTimingBufferSize", maxSize);
        }

        /// <summary>
        /// This event is fired when the browser's resource timing buffer is full.
        /// </summary>
        /// <param name="toDo"></param>
        /// <returns></returns>
        public async Task<IAsyncDisposable> OnResourceTimingBufferFull(Func<Task> toDo)
        {
            return await jsRuntime.AddEventListener(windowRef, "performance", "resourcetimingbufferfull", CallBackInteropWrapper.Create(toDo));
        }

        internal static Type ConvertStringToType(string str)
        {
            return str switch
            {
                "mark" => typeof(PerformanceMark),
                "measure" => typeof(PerformanceMeasure),
                "frame" => typeof(PerformanceFrameTiming),
                "navigation" => typeof(PerformanceNavigationTiming),
                "resource" => typeof(PerformanceResourceTiming),
                "paint" => typeof(PerformancePaintTiming),
                _ => typeof(PerformanceMark)
            };
        }

        internal static string ConvertTypeToString(Type type)
        {
            return type switch
            {
                Type t when t == typeof(PerformanceMark) => "mark",
                Type t when t == typeof(PerformanceMeasure) => "measure",
                Type t when t == typeof(PerformanceFrameTiming) => "frame",
                Type t when t == typeof(PerformanceNavigationTiming) => "navigation",
                Type t when t == typeof(PerformanceResourceTiming) => "resource",
                Type t when t == typeof(PerformancePaintTiming) => "paint",
                _ => null
            };
        }
    }
}