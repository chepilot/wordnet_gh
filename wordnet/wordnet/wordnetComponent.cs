using System;
using System.IO;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using Syn.WordNet;

// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace wordnet
{
    public class wordnetComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        /// 
        WordNetEngine wordNet = new WordNetEngine();

        public wordnetComponent()
          : base("wordnet", "wordnet",
              "Description",
              "wordnet", "wordnet")
        {
            // load wordnet database
            var directory = Directory.GetCurrentDirectory();

            wordNet.AddDataSource(new StreamReader(Path.Combine(directory, "data.adj")), PartOfSpeech.Adjective);
            wordNet.AddDataSource(new StreamReader(Path.Combine(directory, "data.adv")), PartOfSpeech.Adverb);
            wordNet.AddDataSource(new StreamReader(Path.Combine(directory, "data.noun")), PartOfSpeech.Noun);
            wordNet.AddDataSource(new StreamReader(Path.Combine(directory, "data.verb")), PartOfSpeech.Verb);

            wordNet.AddIndexSource(new StreamReader(Path.Combine(directory, "index.adj")), PartOfSpeech.Adjective);
            wordNet.AddIndexSource(new StreamReader(Path.Combine(directory, "index.adv")), PartOfSpeech.Adverb);
            wordNet.AddIndexSource(new StreamReader(Path.Combine(directory, "index.noun")), PartOfSpeech.Noun);
            wordNet.AddIndexSource(new StreamReader(Path.Combine(directory, "index.verb")), PartOfSpeech.Verb);

            //Console.WriteLine("Loading database...");
            wordNet.Load();
            //Console.WriteLine("Load completed.");

        }

        ///// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("String1", "S1", "String", GH_ParamAccess.item);
            pManager.AddTextParameter("String2", "S2", "String", GH_ParamAccess.item);
            pManager[1].Optional = true;
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Synonym", "S", "Synonym", GH_ParamAccess.list);
            pManager.AddTextParameter("Part of Speach", "P", "Part of Speach", GH_ParamAccess.list);
            pManager.AddTextParameter("Gloss", "G", "Gloss", GH_ParamAccess.list);
            pManager.AddTextParameter("Similarity", "S", "Similarity", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {

            string term1 = null;
            string term2 = null;

            if (!DA.GetData(0, ref term1)) { return; }
           

            // If the retrieved data is Nothing, we need to abort.
            // We're also going to abort on a zero-length String.
            if (term1 == null) { return; }
            if (term1.Length == 0) { return; }

            var synSetList = wordNet.GetSynSets(term1);

            if (synSetList.Count == 0) Console.WriteLine($"No SynSet found for '{term1}'");

            var wordList = new List<String>();
            var partOfSpeechList = new List<String>();
            var glossList = new List<String>();


            foreach (var synSet in synSetList)
            {
                var synonym = string.Join(", ", synSet.Words);
                wordList.Add(synonym);
                partOfSpeechList.Add(synSet.PartOfSpeech.ToString());
                glossList.Add(synSet.Gloss);
            }

            if (DA.GetData(1, ref term2))
            {
                var similarity = wordNet.GetSentenceSimilarity(term1, term2);
                DA.SetData(3, similarity);
            }
           
            // Use the DA object to assign a new String to the first output parameter.
            DA.SetDataList(0, wordList);
            DA.SetDataList(1, partOfSpeechList);
            DA.SetDataList(2, glossList);
            
        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                // You can add image files to your project resources and access them like this:
                //return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("fa177114-8729-47b8-92c8-0f4d0cffe3d4"); }
        }
    }
}
