//using Abp.Domain.Services;
//using Microsoft.ML;
//using Microsoft.ML.Data;
//using Microsoft.ML.Trainers;
//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.IO;
//using System.Linq;


//namespace Rating.ProductRecommender
//{
//    public class ProductRecommender : IDomainService
//    {
//        private static string BaseDataSetRelativePath = @"../../../Data";
//        private static string TrainingDataRelativePath = $"{BaseDataSetRelativePath}/Amazon0302.txt";
//        private static string TrainingDataLocation = GetAbsolutePath(TrainingDataRelativePath);

//        private static string BaseModelRelativePath = @"../../../Model";
//        private static string ModelRelativePath = $"{BaseModelRelativePath}/model.zip";
//        private static string ModelPath = GetAbsolutePath(ModelRelativePath);

//        static void Main(string[] args)
//        {
//            //Create MLContext to be shared across the model creation workflow objects 
//            MLContext mlContext = new MLContext();

//            //Read the trained data using TextLoader by defining the schema for reading the product co-purchase dataset
//            var traindata = mlContext.Data.LoadFromTextFile(path: TrainingDataLocation,
//                                                      columns: new[]
//                                                                {
//                                                                    new TextLoader.Column("Label", DataKind.Single, 0),
//                                                                    new TextLoader.Column(name:nameof(ProductEntry.ProductID), dataKind:DataKind.UInt32, source: new [] { new TextLoader.Range(0) }, keyCount: new KeyCount(262111)),
//                                                                    new TextLoader.Column(name:nameof(ProductEntry.CoPurchaseProductID), dataKind:DataKind.UInt32, source: new [] { new TextLoader.Range(1) }, keyCount: new KeyCount(262111))
//                                                                },
//                                                      hasHeader: true,
//                                                      separatorChar: '\t');

//            //        LossFunction, Alpa, Lambda and a few others like K and C as shown below and call the trainer. 
//            var options = new MatrixFactorizationTrainer.Options
//            {
//                MatrixColumnIndexColumnName = "userIdEncoded",
//                MatrixRowIndexColumnName = "movieIdEncoded",
//                LabelColumnName = "Label",
//                NumberOfIterations = 20,
//                ApproximationRank = 100
//            };

//            var trainerEstimator = estimator.Append(mlContext.Recommendation().Trainers.MatrixFactorization(options));

//            //Train the model fitting to the DataSet
//            ITransformer model = est.Fit(traindata);

//            IDataView transformedData = model.Transform(traindata);

//            var trainedDataTable = transformedData.ToDataTable();


//            var coPurchashedProductIDS = transformedData.GetColumn<uint>("CoPurchaseProductID").ToArray();

//            // Create prediction engine and predict the score for Product 63 being co-purchased with Product 3.
//            var predictionengine = mlContext.Model.CreatePredictionEngine<ProductEntry, Copurchase_prediction>(model);


//            Console.WriteLine($"For ProductID = 3 , The recommendad products are : ");


//            List<uint> allRecommendedProducts = new List<uint>();

//            foreach (var recommendedProduct in coPurchashedProductIDS)
//            {
//                var prediction = predictionengine.Predict(
//                new ProductEntry()
//                {
//                    ProductID = 3,
//                    CoPurchaseProductID = recommendedProduct
//                });

//                var score = Math.Round(prediction.Score, 1);

//                if (score >= 0.5f)
//                    allRecommendedProducts.Add(recommendedProduct);
//            }

//            allRecommendedProducts = allRecommendedProducts.Distinct().ToList();

//            allRecommendedProducts.Sort((a, b) => b.CompareTo(a));

//            for (int i = 0; i < 5; i++)
//                Console.WriteLine($"Product ID : {allRecommendedProducts[i]}");




//            Console.WriteLine("=============== End of process, hit any key to finish ===============");
//            Console.ReadKey();
//        }

//        public static string GetAbsolutePath(string relativeDatasetPath)
//        {
//            FileInfo _dataRoot = new FileInfo(typeof(ProductRecommender).Assembly.Location);
//            string assemblyFolderPath = _dataRoot.Directory.FullName;

//            string fullPath = Path.Combine(assemblyFolderPath, relativeDatasetPath);

//            return fullPath;
//        }

//        public class Copurchase_prediction
//        {
//            public float Score { get; set; }
//        }

//        public class ProductEntry
//        {
//            [KeyType(count: 262111)]
//            public uint ProductID { get; set; }

//            [KeyType(count: 262111)]
//            public uint CoPurchaseProductID { get; set; }
//        }

//        public static (IDataView training, IDataView test) LoadData(MLContext mlContext)
//        {
//            var trainingDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "recommendation-ratings-train.csv");
//            var testDataPath = Path.Combine(Environment.CurrentDirectory, "Data", "recommendation-ratings-test.csv");

//            IDataView trainingDataView = mlContext.Data.LoadFromTextFile<MovieRating>(trainingDataPath, hasHeader: true, separatorChar: ',');
//            IDataView testDataView = mlContext.Data.LoadFromTextFile<MovieRating>(testDataPath, hasHeader: true, separatorChar: ',');

//            return (trainingDataView, testDataView);

//        }
//    }
//}
