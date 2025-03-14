using CsvHelper;
using FellowOakDicom;

using var w = new CsvWriter(Console.Out, System.Globalization.CultureInfo.InvariantCulture);
w.WriteRecords(Directory.EnumerateFiles(".", "*.dcm", new EnumerationOptions
{
    MatchCasing = MatchCasing.CaseInsensitive,
    IgnoreInaccessible = true,
    RecurseSubdirectories = true,
    ReturnSpecialDirectories = false
}).SelectMany(static dcm => DicomFile.Open(dcm).Dataset.SelectMany(t => Entry.ProcessTag(dcm, t))));

internal sealed class Entry
{
    public string Id { get; }
    public string Name { get; }
    public string Value { get; }

    private Entry(string id, string name, string value)
    {
        Id = id;
        Name = name;
        Value = value;
    }

    public static IEnumerable<Entry> ProcessTag(string id, DicomItem item)
    {
        return item switch
        {
            DicomAttributeTag aTag => aTag.Values.Select(v => new Entry(id, aTag.Tag.DictionaryEntry.Name, v.DictionaryEntry.Name)),
            DicomStringElement s => StringEntries(id, s.Tag.DictionaryEntry.Name, s),
            DicomSequence seq => seq.Items.SelectMany(ds => ds.SelectMany(i => ProcessTag(id, i))),
            _ => [new Entry(id, item.Tag.DictionaryEntry.Name, item.ToString())]
        };
    }

    private static IEnumerable<Entry> StringEntries(string id, string tag, DicomStringElement e)
    {
        for (var i = 0; i < e.Count; i++)
            yield return new Entry(id, tag, e.Get<string>(i));
    }
}
