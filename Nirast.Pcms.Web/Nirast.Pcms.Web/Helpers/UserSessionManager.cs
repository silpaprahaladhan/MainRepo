using Nirast.Pcms.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Nirast.Pcms.Web.Helpers
{
    public class UserSessionManager
    {


        public static void SetSearchedCareTaker(Controller controller, List<SearchedCareTakers> objSearchedCareTakers)
        {
            controller.Session["SearchedCareTakers"] = objSearchedCareTakers;
        }

        public static List<SearchedCareTakers> GetSearchedCareTaker(Controller controller)
        {
            List<SearchedCareTakers> objSearchedCareTakers = (List<SearchedCareTakers>)controller.Session["SearchedCareTakers"];
            if (null == objSearchedCareTakers)
                objSearchedCareTakers = new List<SearchedCareTakers>();
            return objSearchedCareTakers;
        }

        /// <summary>
        /// Set the CareTakerExperience to session
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="objCareTakerExperienceSave"></param>
        public static void SetCareTakerExperience(Controller controller, List<CaretakerExperiences> objCareTakerExperiences)
        {
            controller.Session["CareTakerExperience"] = objCareTakerExperiences;
        }

        /// <summary>
        /// Set the CareTakerExperience to session
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="objCareTakerExperienceSave"></param>
        public static void SetCareTakerDocuments(Controller controller, List<DocumentsList> objCareTakerDocuments,string sessionName)
        {
            controller.Session[sessionName] = objCareTakerDocuments;
        }

        /// <summary>
        /// Get the CareTakerExperience from session
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static List<CaretakerExperiences> GetCareTakerExperience(Controller controller)
        {
            List<CaretakerExperiences> objCareTakerExperience = (List<CaretakerExperiences>)controller.Session["CareTakerExperience"];
            if (null == objCareTakerExperience)
                objCareTakerExperience = new List<CaretakerExperiences>();
            return objCareTakerExperience;
        }

        /// <summary>
        /// Get the CareTakerExperience from session
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static List<DocumentsList> GetCareTakerDocuments(Controller controller , string sessionName)
        {
            List<DocumentsList> objCareTakerCertificates = (List<DocumentsList>)controller.Session[sessionName];
            if (null == objCareTakerCertificates)
                objCareTakerCertificates = new List<DocumentsList>();
            return objCareTakerCertificates;
        }

        /// <summary>
        /// Add the CareTakerExperience to CareTakerRegistrationViewModel
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="objCareTakerRegistrationViewModel"></param>
        public static void AddCareTakerExperience(Controller controller, CaretakerExperiences objExperience)
        {
            List<CaretakerExperiences> objCareTakerExperience = GetCareTakerExperience(controller);
            objCareTakerExperience.Add(objExperience);
            SetCareTakerExperience(controller, objCareTakerExperience);
        }

        /// <summary>
        /// Add the CareTakerExperience to CareTakerRegistrationViewModel
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="objCareTakerRegistrationViewModel"></param>
        public static void AddCareTakerDocuments(Controller controller, List<DocumentsList> objDocument, string sessionName)
        {
            List<DocumentsList> objDocumentList = new List<DocumentsList>();
            objDocumentList.AddRange(objDocument);
            SetCareTakerDocuments(controller, objDocumentList, sessionName);
        }

        /// <summary>
        /// Set the CareTakerQualification to session
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="objCareTakerQualifications"></param>
        public static void SetCareTakerQualification(Controller controller, List<CareTakerQualifications> objCareTakerQualificationSave)
        {
            controller.Session["CareTakerQualifications"] = objCareTakerQualificationSave;
        }

        /// <summary>
        /// Get the CareTakerQualification from session
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static List<CareTakerQualifications> GetCareTakerQualification(Controller controller)
        {
            List<CareTakerQualifications> objCareTakerQualificationSave = (List<CareTakerQualifications>)controller.Session["CareTakerQualifications"];
            if (null == objCareTakerQualificationSave)
                objCareTakerQualificationSave = new List<CareTakerQualifications>();
            return objCareTakerQualificationSave;
        }

        /// <summary>
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="objQualification"></param>
        public static void AddCareTakerQualification(Controller controller, CareTakerQualifications objQualification)
        {
            List<CareTakerQualifications> objCareTakerQualificationSave = GetCareTakerQualification(controller);
            objCareTakerQualificationSave.Add(objQualification);
            SetCareTakerQualification(controller, objCareTakerQualificationSave);
        }



        /// <summary>
        /// Set the CareTakerService to session
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="objCareTakerService"></param>
        public static void SetCareTakerService(Controller controller, List<CareTakerServices> objCareTakerService)
        {
            controller.Session["CareTakerService"] = objCareTakerService;
        }

        /// <summary>
        /// Get the CareTakerService from session
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static List<CareTakerServices> GetCareTakerService(Controller controller)
        {
            List<CareTakerServices> objCareTakerService = (List<CareTakerServices>)controller.Session["CareTakerService"];
            if (null == objCareTakerService)
                objCareTakerService = new List<CareTakerServices>();
            return objCareTakerService;
        }

        /// <summary>
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="objService"></param>
        public static void AddCareTakerService(Controller controller, CareTakerServices objService)
        {
            List<CareTakerServices> objCareTakerService = GetCareTakerService(controller);
            objCareTakerService.Add(objService);
            SetCareTakerService(controller, objCareTakerService);
        }

        public static void SetApprovedRates(Controller controller, List<CareTakerServices> objCareTakerService)
        {
            controller.Session["CareTakerApprovedRates"] = objCareTakerService;
        }

        //public static void AddApprovedRates(Controller controller, CareTakerServices objService)
        //{
        //    List<CareTakerServices> objCareTakerService = GetApprovedRates(controller);
        //    objCareTakerService.Add(objService);
        //    SetApprovedRates(controller, objCareTakerService);
        //}

        //public static List<CareTakerServices> GetApprovedRates(Controller controller)
        //{
        //    List<CareTakerServices> objCareTakerService = (List<CareTakerServices>)controller.Session["CareTakerApprovedRates"];
        //    if (null == objCareTakerService)
        //        objCareTakerService = new List<CareTakerServices>();
        //    return objCareTakerService;
        //}

        /// <summary>
        /// Set the CareTakerService to session
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="objCareTakerClient"></param>
        public static void SetCareTakerClient(Controller controller, List<CareTakerClients> objCareTakerClient)
        {
            controller.Session["CareTakerClient"] = objCareTakerClient;
        }

        /// <summary>
        /// Get the CareTakerClient from session
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static List<CareTakerClients> GetCareTakerClient(Controller controller)
        {
            List<CareTakerClients> objCareTakerClient = (List<CareTakerClients>)controller.Session["CareTakerClient"];
            if (null == objCareTakerClient)
                objCareTakerClient = new List<CareTakerClients>();
            return objCareTakerClient;
        }

        /// <summary>
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="objClient"></param>
        public static void AddCareTakerClient(Controller controller, CareTakerClients objClient)
        {
            List<CareTakerClients> objCareTakerClient = GetCareTakerClient(controller);
            objCareTakerClient.Add(objClient);
            SetCareTakerClient(controller, objCareTakerClient);
        }

        ///// <summary>
        ///// Set the CareTakerOrientation to session
        ///// </summary>
        ///// <param name="controller"></param>
        ///// <param name="objCareTakerOrientation"></param>
        //public static void SetCareTakerOrientation(Controller controller, List<CareTakerOrientations> objCareTakerOrientation)
        //{
        //    controller.Session["CareTakerOrientation"] = objCareTakerOrientation;
        //}

        ///// <summary>
        ///// Get the CareTakerOrientation from session
        ///// </summary>
        ///// <param name="controller"></param>
        ///// <returns></returns>
        //public static List<CareTakerOrientations> GetCareTakerOrientation(Controller controller)
        //{
        //    List<CareTakerOrientations> objCareTakerOrientation = (List<CareTakerOrientations>)controller.Session["CareTakerOrientation"];
        //    if (null == objCareTakerOrientation)
        //        objCareTakerOrientation = new List<CareTakerOrientations>();
        //    return objCareTakerOrientation;
        //}

        ///// <summary>
        ///// </summary>
        ///// <param name="controller"></param>
        ///// <param name="objOrientation"></param>
        //public static void AddCareTakerOrientation(Controller controller, CareTakerOrientations objOrientation)
        //{
        //    List<CareTakerOrientations> objCareTakerOrientation = GetCareTakerOrientation(controller);
        //    objCareTakerOrientation.Add(objOrientation);
        //    SetCareTakerOrientation(controller, objCareTakerOrientation);
        //}

        /// <summary>
        /// Get the ClientTimeShift from session
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static List<ClientShiftDetails> GetClientTimeShift(Controller controller)
        {
            List<ClientShiftDetails> objTimeShiftViewModelSave = (List<ClientShiftDetails>)controller.Session["ClientTimeShiftDetails"];
            if (null == objTimeShiftViewModelSave)
                objTimeShiftViewModelSave = new List<ClientShiftDetails>();
            return objTimeShiftViewModelSave;
        }

        /// <summary>
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="objTimeShift"></param>
        public static void AddClientTimeShift(Controller controller, ClientShiftDetails objTimeShift)
        {
            List<ClientShiftDetails> objTimeShiftList = GetClientTimeShift(controller);
            objTimeShiftList.Add(objTimeShift);
            SetClientTimeShift(controller, objTimeShiftList);
        }
        /// <summary>
        /// Set the ClientShifDetails to session
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="objTimeShiftList"></param>
        public static void SetClientTimeShift(Controller controller, List<ClientShiftDetails> objTimeShiftList)
        {
            controller.Session["ClientTimeShiftDetails"] = objTimeShiftList;
        }

        /// <summary>
        /// Get the caretakers from session
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static List<ClientCaretakers> GetCaretakersList(Controller controller)
        {
            List<ClientCaretakers> objCareTakersSave = (List<ClientCaretakers>)controller.Session["ClientCareTakersList"];
            if (null == objCareTakersSave)
                objCareTakersSave = new List<ClientCaretakers>();
            return objCareTakersSave;
        }


        /// <summary>
        /// Get the category and rate from session
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public static List<ClientCategoryRate> GetCategoryRateList(Controller controller)
        {
            List<ClientCategoryRate> objCareTakersSave = (List<ClientCategoryRate>)controller.Session["ClientCategoryRateList"];
            if (null == objCareTakersSave)
                objCareTakersSave = new List<ClientCategoryRate>();
            return objCareTakersSave;
        }

        /// <summary>
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="objCareTakers"></param>
        public static void AddCaretakersList(Controller controller, ClientCaretakers objCareTakers)
        {
            List<ClientCaretakers> objCareTakersList = GetCaretakersList(controller);
            objCareTakersList.Add(objCareTakers);
            SetCaretakersList(controller, objCareTakersList);
        }

        /// <summary>
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="objCareTakers"></param>
        public static void AddCategoryRateList(Controller controller, ClientCategoryRate objCareTakers)
        {
            List<ClientCategoryRate> objCareTakersList = GetCategoryRateList(controller);
            ClientCategoryRate objCareTakerListSaearch = objCareTakersList.Find(x => x.CategoryId == objCareTakers.CategoryId);
            if(null== objCareTakerListSaearch)
            {
                objCareTakersList.Add(objCareTakers);
            }
            else
            {
                objCareTakersList.RemoveAll(x => x.CategoryId == objCareTakers.CategoryId);
                objCareTakersList.Add(objCareTakers);
            }
            
            SetCategoryRateList(controller, objCareTakersList);
        }
        /// <summary>
        /// Set the ClientShifDetails to session
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="objCareTakersList"></param>
        public static void SetCaretakersList(Controller controller, List<ClientCaretakers> objCareTakersList)
        {
            controller.Session["ClientCareTakersList"] = objCareTakersList;
        }

        /// <summary>
        /// Set the Category and rate to session
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="objCareTakersList"></param>
        public static void SetCategoryRateList(Controller controller, List<ClientCategoryRate> objCareTakersList)
        {
            controller.Session["ClientCategoryRateList"] = objCareTakersList;
        }
    }
}