/*
This is a useful method to see what environment variables your application can see.
*/
        public class EnvironmentData
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class EnvironmentStrings
        {
            public List<EnvironmentData> EnvironmentValues { get; set; }
        }

		public EnvironmentStrings DumpEnvironment(string filterStr = "")
        {
            Dictionary<string, string> environment = new Dictionary<string, string>();
            var environmentData = new EnvironmentStrings
            {
                EnvironmentValues = new List<EnvironmentData>()
            };
			
			const string defaultFilter = "-";
            var filter = string.IsNullOrEmpty(filterStr) ? defaultFilter : filterStr;
            int longestKey = 0;

            foreach (DictionaryEntry e in System.Environment.GetEnvironmentVariables())
            {
                var key = e.Key;
                var value = e.Value;
				// If the key or the value is null, skip it.
                if (key == null || value == null) continue;
                if (key.ToString().Contains(filter))
                {
                    environment.Add(key.ToString(), value.ToString());
                    longestKey = (key.ToString().Length > longestKey) ? key.ToString().Length : longestKey;
                }
            }

            var sortedList = environment.Keys.OrderBy(k => k.ToString());
            foreach (var item in sortedList)
            {
                var itemStr = PadRight(item, longestKey);
                environmentData.EnvironmentValues.Add(new EnvironmentData
                {
                    Name = itemStr,
                    Value = environment[item]
                });
				// If running in a console app, you could just dump things out to the console screen.
                //Console.WriteLine($"{itemStr} {environment[item]}");
            }

            return environmentData;
        }

		// This method can be found in the UtilStringFunctions class but is here for convenience
        private string PadRight(string data, int length, char character = ' ')
        {
            data = data ?? string.Empty;
            return (data.Length <= length)   // Less than or equal to
                ? data.PadRight(length, character)
                : data.Substring(0, length);
        }
