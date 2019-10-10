using System;
using System.IO;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

using ConcepnetDotNet;
using conceptnet;


// In order to load the result of this wizard, you will also need to
// add the output bin/ folder of this project to the list of loaded
// folder in Grasshopper.
// You can use the _GrasshopperDeveloperSettings Rhino command for that.

namespace conceptnet
{
    public class conceptnetComponent : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        /// 
        ConceptNetWrapper wrapper = new ConceptNetWrapper("http://api.conceptnet.io/");

        public conceptnetComponent()
          : base("conceptnet", "conceptnet",
              "Description",
              "conceptnet", "conceptnet")
        {

        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("Term1", "T1", "String", GH_ParamAccess.item);
            pManager.AddTextParameter("Term2", "T2", "String", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Concept Relations", "CS", "String", GH_ParamAccess.item);
            pManager.AddTextParameter("Related Terms", "RT", "String", GH_ParamAccess.item);
            pManager.AddTextParameter("Terms Relations", "TR", "Terms Relations", GH_ParamAccess.item);
            pManager.AddTextParameter("How Terms Related", "HTR", "String", GH_ParamAccess.item);
            pManager.AddTextParameter("Relation Score", "RS", "String", GH_ParamAccess.item);
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

            if (!DA.GetData(0, ref term1) || !DA.GetData(1, ref term2)) { return; }

            // If the retrieved data is Nothing, we need to abort.
            // We're also going to abort on a zero-length String.
            if ((term1 == null) || (term2 == null)){ return; }
            if ((term1.Length == 0) || (term2.Length == 0) ){ return; }


            

            // relations as string
            var conceptRelations = wrapper.GetConceptRelationsAsString(term1);

            // Return terms related to the term
            var relatedTerms = wrapper.GetRelatedTerms("Tea Kettle");

            // Return the relations between these two terms
            // var result3 = wrapper.GetConceptNetResults("dog", "bark");

            // Return the relations between these two terms as comma separated string
            var termsRelations = wrapper.GetConceptRelationsAsString(term1, term2);

            // Return the relations between these two terms in data table format 
            // var result5 = wrapper.GetConceptRelationsAsDataTable("dog", "bark");

            // If you just want to see how related term1 (e.g., "tea kettle") is to term2 (e.g., "coffee pot") ** 
            var howTermsRelated = wrapper.GetHowTermsRelated(term1, term2);
            
               // Return the relation score 
               var relationScore= wrapper.GetRelationScore(term1, term2);

            // Get results based on query (comma seperated string ) 
            // var result9 = wrapper.GetConceptNetQueryResults("node=/c/en/dog,other=/c/en/bark");

            DA.SetData(0, conceptRelations);
            DA.SetData(1, relatedTerms);
            DA.SetData(2, termsRelations);
            DA.SetData(3, howTermsRelated);
            DA.SetData(4, relationScore);
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
            get { return new Guid("fa177114-8729-47b8-92c8-0f4d0cffe3d3"); }
        }
    }
}
