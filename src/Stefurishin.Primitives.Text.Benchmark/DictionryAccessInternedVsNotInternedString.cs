using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Stefurishin.Primitives.Text.Benchmark
{
	/// <summary>
	/// Benchmark to compare Dictionary.Contains() + Dictionary.get[] vs Dictionary.TryGetValue() to retrieve values
	/// </summary>
	[SimpleJob(RuntimeMoniker.Net461, baseline: true)]
	[SimpleJob(RuntimeMoniker.NetCoreApp30)]
	[CsvExporter]
	[RPlotExporter]
	[MarkdownExporterAttribute.StackOverflow]
	[MarkdownExporterAttribute.GitHub]
	[MarkdownExporterAttribute.Atlassian]
	public class DictionryAccessInternedVsNotInternedString
	{
		static void Main() => BenchmarkRunner.Run<DictionryAccessInternedVsNotInternedString>();

		private string _string;
		private InternedString _internedString;
		private Int32 _integer = -1;

		private Dictionary<string, string> _stringDictionary = new Dictionary<string, string>();
		private Dictionary<InternedString, InternedString> _internedStringDictionry = new Dictionary<InternedString, InternedString>();
		private Dictionary<Int32, Int32?> _Int32Dictionry;

		[Params(
				2 << 1,
				2 << 2,
				//2 << 3,
				2 << 4,
				2 << 5,
				//2 << 6,
				2 << 7,
				//2 << 8,
				//2 << 9,
				2 << 10,
				//2 << 11,
				//2 << 12,
				//2 << 13,
				//2 << 14,
				//2 << 15,
				2 << 16
			)]
		public int StringLength;

		[Params(
				//2 << 1,
				//2 << 2,
				//2 << 3,
				//2 << 4,
				//2 << 5,
				2 << 6
				//2 << 7,
				//2 << 8,
				//2 << 9,
				//2 << 10,
				//2 << 11,
				//2 << 12,
				//2 << 13,
				//2 << 14,
				//2 << 15,
				//2 << 16
			)]
		public int DictionarySize;

		[GlobalSetup]
		public void Setup()
		{
			// we don't care about the size of collections
			// but it should contain SOME items, to be sure it's not optimizing the lookup
			foreach (var rs in GenerateRandomStrings(DictionarySize))
			{
				_stringDictionary.Add(rs, rs);
				var irs = new InternedString(rs);
				_internedStringDictionry.Add(irs, irs);
			}

			_string = new string(Enumerable.Repeat('x', StringLength).ToArray());
			_internedString = new InternedString(_string);

			_stringDictionary.Add(_string, _string);
			_internedStringDictionry.Add(_internedString, _internedString);
			_Int32Dictionry = Enumerable.Range(1, StringLength * DictionarySize).ToDictionary(k => k, v => (int?)v);
			_Int32Dictionry.Add(_integer, _integer);
		}

		[Benchmark]
		public string RetrieveStringByContainsKey()
		{
			if (_stringDictionary.ContainsKey(_string))
			{
				return _stringDictionary[_string];
			}
			return null;
		}

		[Benchmark]
		public string RetrieveStringByTryGet()
		{
			_stringDictionary.TryGetValue(_string, out var x);
			return x;
		}

		[Benchmark]
		public InternedString RetrieveInternedStringByContainsKey()
		{
			if (_internedStringDictionry.ContainsKey(_internedString))
			{
				return _internedStringDictionry[_internedString];
			}
			return null;
		}

		[Benchmark]
		public InternedString RetrieveInternedStringByTryGet()
		{
			_internedStringDictionry.TryGetValue(_internedString, out var x);
			return x;
		}

		[Benchmark]
		public int? RetrieveInt32ByContainsKey()
		{
			if (_Int32Dictionry.ContainsKey(_integer))
			{
				return _Int32Dictionry[_integer];
			}
			return null;
		}

		[Benchmark]
		public int? RetrieveInt32ByTryGet()
		{
			_Int32Dictionry.TryGetValue(_integer, out var x);
			return x;
		}



		private static IEnumerable<string> GenerateRandomStrings(int count)
		{
			for (int i = 0; i < count; i++)
				yield return Guid.NewGuid().ToString();
		}
	}
}
