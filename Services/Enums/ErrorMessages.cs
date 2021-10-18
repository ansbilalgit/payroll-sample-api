using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Enums
{
    public enum ErrorMessages
    {
        #region Application
        EXCEPTION_MESSAGE,
        ERROR_OCCURED_ON_UPDATING,
        UNABLE_TO_CONVERT_TO_PDF,
        INTERNAL_SERVER_ERROR,
        ROLE_ALREADY_EXIST,
        #endregion

        #region User
        USER_AREA_DOES_NOT_EXIST,
        USER_CARD_NOT_EXIST,
        USER_CARD_EXPIRED,
        USER_PROFILE_NOT_EXIST,
        USER_PROFILE_ALREADY_EXIST,
        USER_CARD_INVALID,
        USER_EMAIL_NOT_FOUND,
        USER_EMAIL_ALREADY_EXIST,
        USER_NO_LATEST_AREA_EXIST,
        USER_INVALID_VERIFICATION_CODE,
        USER_NOT_EXIST,
        USER_INVALID_USERNAME_PASSWORD,
        USER_ALREADY_EXIST,
        USER_MOBILE_NUMBER_ALREADY_EXIST,
        USER_NOT_CREATED,
        USER_NOT_ADDED_TO_ROLE,
        USER_INVALID_CURRENT_PASSWORD,
        USER_NOT_SUCCESSFULLY_REGISTERED,
        USER_SETTING_NOT_EXIST,
        UNABLE_TO_DELETE_USER,
        DEVICE_NOT_FOUND,
        USER_CARD_CANNOT_BE_DELETED_BECAUSE_USED_IN_PARKING,
        USER_SETTING_INCORRECT_ATTRIBUTE_KEY,
        USER_IS_DISABLED,
        USER_ALREADY_EXIST_ON_MOBILE,
        COURSES_NOT_AVAILABLE
        #endregion
    }
}
