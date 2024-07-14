using Compiler.IO;
using Compiler.Tokenization;
using Compiler.SyntacticAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using static System.Console;
using Compiler.Nodes;

namespace Compiler
{

    public class Compiler
    {

        public ErrorReporter Reporter { get; }

        public IFileReader Reader { get; }

        public Tokenizer Tokenizer { get; }

        public Parser Parser { get; }

        public Compiler(string inputFile)
        {

            Reporter = new ErrorReporter();
            Reader = new FileReader(inputFile);
            Tokenizer = new Tokenizer(Reader, Reporter);
            Parser = new Parser(Reporter);

        }


        public void Compile()
        {

            //Tokenize
            Write("Tokenising...");
            List<Token> tokens = Tokenizer.GetAllTokens();
            if (Reporter.HasErrors) return;
            WriteLine("Done");


            // Old Parser
            //Write("Parsing...");
            //Parser.Parse(tokens);
            //if (Reporter.HasErrors) return;
            //WriteLine("Done");

            //New Parser
            Write("Parsing...");
            ProgramNode tree = Parser.Parse(tokens);              
            if (Reporter.HasErrors) return;
            WriteLine("Done");

            //Generate Tree
            WriteLine(TreePrinter.ToString(tree));                

        }

        private void WriteFinalMessage()
        {
            WriteLine("Compilation completed successfully!");
        }


        public static void Main(string[] args)
        {
            if (args == null || args.Length != 1 || args[0] == null)
                WriteLine("ERROR: Must call the program with exactly one argument, the input file (*.sq)");
            else if (!File.Exists(args[0]))
                WriteLine($"ERROR: The input file \"{Path.GetFullPath(args[0])}\" does not exist");
            else
            {
                string inputFile = args[0];
                Compiler compiler = new Compiler(inputFile);
                WriteLine("Compiling...");
                compiler.Compile();
                compiler.WriteFinalMessage();
            }

        }


    }

}
