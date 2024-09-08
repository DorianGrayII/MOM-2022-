// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// LitJson.JsonReader
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using LitJson;

public class JsonReader
{
    private static readonly IDictionary<int, IDictionary<int, int[]>> parse_table;

    private Stack<int> automaton_stack;

    private int current_input;

    private int current_symbol;

    private bool end_of_json;

    private bool end_of_input;

    private Lexer lexer;

    private bool parser_in_string;

    private bool parser_return;

    private bool read_started;

    private TextReader reader;

    private bool reader_is_owned;

    private bool skip_non_members;

    private object token_value;

    private JsonToken token;

    public bool AllowComments
    {
        get
        {
            return this.lexer.AllowComments;
        }
        set
        {
            this.lexer.AllowComments = value;
        }
    }

    public bool AllowSingleQuotedStrings
    {
        get
        {
            return this.lexer.AllowSingleQuotedStrings;
        }
        set
        {
            this.lexer.AllowSingleQuotedStrings = value;
        }
    }

    public bool SkipNonMembers
    {
        get
        {
            return this.skip_non_members;
        }
        set
        {
            this.skip_non_members = value;
        }
    }

    public bool EndOfInput => this.end_of_input;

    public bool EndOfJson => this.end_of_json;

    public JsonToken Token => this.token;

    public object Value => this.token_value;

    static JsonReader()
    {
        JsonReader.parse_table = JsonReader.PopulateParseTable();
    }

    public JsonReader(string json_text)
        : this(new StringReader(json_text), owned: true)
    {
    }

    public JsonReader(TextReader reader)
        : this(reader, owned: false)
    {
    }

    private JsonReader(TextReader reader, bool owned)
    {
        if (reader == null)
        {
            throw new ArgumentNullException("reader");
        }
        this.parser_in_string = false;
        this.parser_return = false;
        this.read_started = false;
        this.automaton_stack = new Stack<int>();
        this.automaton_stack.Push(65553);
        this.automaton_stack.Push(65543);
        this.lexer = new Lexer(reader);
        this.end_of_input = false;
        this.end_of_json = false;
        this.skip_non_members = true;
        this.reader = reader;
        this.reader_is_owned = owned;
    }

    private static IDictionary<int, IDictionary<int, int[]>> PopulateParseTable()
    {
        IDictionary<int, IDictionary<int, int[]>> result = new Dictionary<int, IDictionary<int, int[]>>();
        JsonReader.TableAddRow(result, ParserToken.Array);
        JsonReader.TableAddCol(result, ParserToken.Array, 91, 91, 65549);
        JsonReader.TableAddRow(result, ParserToken.ArrayPrime);
        JsonReader.TableAddCol(result, ParserToken.ArrayPrime, 34, 65550, 65551, 93);
        JsonReader.TableAddCol(result, ParserToken.ArrayPrime, 91, 65550, 65551, 93);
        JsonReader.TableAddCol(result, ParserToken.ArrayPrime, 93, 93);
        JsonReader.TableAddCol(result, ParserToken.ArrayPrime, 123, 65550, 65551, 93);
        JsonReader.TableAddCol(result, ParserToken.ArrayPrime, 65537, 65550, 65551, 93);
        JsonReader.TableAddCol(result, ParserToken.ArrayPrime, 65538, 65550, 65551, 93);
        JsonReader.TableAddCol(result, ParserToken.ArrayPrime, 65539, 65550, 65551, 93);
        JsonReader.TableAddCol(result, ParserToken.ArrayPrime, 65540, 65550, 65551, 93);
        JsonReader.TableAddRow(result, ParserToken.Object);
        JsonReader.TableAddCol(result, ParserToken.Object, 123, 123, 65545);
        JsonReader.TableAddRow(result, ParserToken.ObjectPrime);
        JsonReader.TableAddCol(result, ParserToken.ObjectPrime, 34, 65546, 65547, 125);
        JsonReader.TableAddCol(result, ParserToken.ObjectPrime, 125, 125);
        JsonReader.TableAddRow(result, ParserToken.Pair);
        JsonReader.TableAddCol(result, ParserToken.Pair, 34, 65552, 58, 65550);
        JsonReader.TableAddRow(result, ParserToken.PairRest);
        JsonReader.TableAddCol(result, ParserToken.PairRest, 44, 44, 65546, 65547);
        JsonReader.TableAddCol(result, ParserToken.PairRest, 125, 65554);
        JsonReader.TableAddRow(result, ParserToken.String);
        JsonReader.TableAddCol(result, ParserToken.String, 34, 34, 65541, 34);
        JsonReader.TableAddRow(result, ParserToken.Text);
        JsonReader.TableAddCol(result, ParserToken.Text, 91, 65548);
        JsonReader.TableAddCol(result, ParserToken.Text, 123, 65544);
        JsonReader.TableAddRow(result, ParserToken.Value);
        JsonReader.TableAddCol(result, ParserToken.Value, 34, 65552);
        JsonReader.TableAddCol(result, ParserToken.Value, 91, 65548);
        JsonReader.TableAddCol(result, ParserToken.Value, 123, 65544);
        JsonReader.TableAddCol(result, ParserToken.Value, 65537, 65537);
        JsonReader.TableAddCol(result, ParserToken.Value, 65538, 65538);
        JsonReader.TableAddCol(result, ParserToken.Value, 65539, 65539);
        JsonReader.TableAddCol(result, ParserToken.Value, 65540, 65540);
        JsonReader.TableAddRow(result, ParserToken.ValueRest);
        JsonReader.TableAddCol(result, ParserToken.ValueRest, 44, 44, 65550, 65551);
        JsonReader.TableAddCol(result, ParserToken.ValueRest, 93, 65554);
        return result;
    }

    private static void TableAddCol(IDictionary<int, IDictionary<int, int[]>> parse_table, ParserToken row, int col, params int[] symbols)
    {
        parse_table[(int)row].Add(col, symbols);
    }

    private static void TableAddRow(IDictionary<int, IDictionary<int, int[]>> parse_table, ParserToken rule)
    {
        parse_table.Add((int)rule, new Dictionary<int, int[]>());
    }

    private void ProcessNumber(string number)
    {
        int result2;
        long result3;
        ulong result4;
        if ((number.IndexOf('.') != -1 || number.IndexOf('e') != -1 || number.IndexOf('E') != -1) && double.TryParse(number, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        {
            this.token = JsonToken.Double;
            this.token_value = result;
        }
        else if (int.TryParse(number, NumberStyles.Integer, CultureInfo.InvariantCulture, out result2))
        {
            this.token = JsonToken.Int;
            this.token_value = result2;
        }
        else if (long.TryParse(number, NumberStyles.Integer, CultureInfo.InvariantCulture, out result3))
        {
            this.token = JsonToken.Long;
            this.token_value = result3;
        }
        else if (ulong.TryParse(number, NumberStyles.Integer, CultureInfo.InvariantCulture, out result4))
        {
            this.token = JsonToken.Long;
            this.token_value = result4;
        }
        else
        {
            this.token = JsonToken.Int;
            this.token_value = 0;
        }
    }

    private void ProcessSymbol()
    {
        if (this.current_symbol == 91)
        {
            this.token = JsonToken.ArrayStart;
            this.parser_return = true;
        }
        else if (this.current_symbol == 93)
        {
            this.token = JsonToken.ArrayEnd;
            this.parser_return = true;
        }
        else if (this.current_symbol == 123)
        {
            this.token = JsonToken.ObjectStart;
            this.parser_return = true;
        }
        else if (this.current_symbol == 125)
        {
            this.token = JsonToken.ObjectEnd;
            this.parser_return = true;
        }
        else if (this.current_symbol == 34)
        {
            if (this.parser_in_string)
            {
                this.parser_in_string = false;
                this.parser_return = true;
                return;
            }
            if (this.token == JsonToken.None)
            {
                this.token = JsonToken.String;
            }
            this.parser_in_string = true;
        }
        else if (this.current_symbol == 65541)
        {
            this.token_value = this.lexer.StringValue;
        }
        else if (this.current_symbol == 65539)
        {
            this.token = JsonToken.Boolean;
            this.token_value = false;
            this.parser_return = true;
        }
        else if (this.current_symbol == 65540)
        {
            this.token = JsonToken.Null;
            this.parser_return = true;
        }
        else if (this.current_symbol == 65537)
        {
            this.ProcessNumber(this.lexer.StringValue);
            this.parser_return = true;
        }
        else if (this.current_symbol == 65546)
        {
            this.token = JsonToken.PropertyName;
        }
        else if (this.current_symbol == 65538)
        {
            this.token = JsonToken.Boolean;
            this.token_value = true;
            this.parser_return = true;
        }
    }

    private bool ReadToken()
    {
        if (this.end_of_input)
        {
            return false;
        }
        this.lexer.NextToken();
        if (this.lexer.EndOfInput)
        {
            this.Close();
            return false;
        }
        this.current_input = this.lexer.Token;
        return true;
    }

    public void Close()
    {
        if (this.end_of_input)
        {
            return;
        }
        this.end_of_input = true;
        this.end_of_json = true;
        if (this.reader_is_owned)
        {
            using (this.reader)
            {
            }
        }
        this.reader = null;
    }

    public bool Read()
    {
        if (this.end_of_input)
        {
            return false;
        }
        if (this.end_of_json)
        {
            this.end_of_json = false;
            this.automaton_stack.Clear();
            this.automaton_stack.Push(65553);
            this.automaton_stack.Push(65543);
        }
        this.parser_in_string = false;
        this.parser_return = false;
        this.token = JsonToken.None;
        this.token_value = null;
        if (!this.read_started)
        {
            this.read_started = true;
            if (!this.ReadToken())
            {
                return false;
            }
        }
        while (true)
        {
            if (this.parser_return)
            {
                if (this.automaton_stack.Peek() == 65553)
                {
                    this.end_of_json = true;
                }
                return true;
            }
            this.current_symbol = this.automaton_stack.Pop();
            this.ProcessSymbol();
            if (this.current_symbol == this.current_input)
            {
                if (!this.ReadToken())
                {
                    break;
                }
                continue;
            }
            int[] array;
            try
            {
                array = JsonReader.parse_table[this.current_symbol][this.current_input];
            }
            catch (KeyNotFoundException inner_exception)
            {
                throw new JsonException((ParserToken)this.current_input, inner_exception);
            }
            if (array[0] != 65554)
            {
                for (int num = array.Length - 1; num >= 0; num--)
                {
                    this.automaton_stack.Push(array[num]);
                }
            }
        }
        if (this.automaton_stack.Peek() != 65553)
        {
            throw new JsonException("Input doesn't evaluate to proper JSON text");
        }
        if (this.parser_return)
        {
            return true;
        }
        return false;
    }
}
