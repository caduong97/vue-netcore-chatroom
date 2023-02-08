
interface IRegisterPasswordUserRequest {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string | null;
  lastName: string | null
}

export default class RegisterPasswordUserRequest implements IRegisterPasswordUserRequest {
  email: string = "";
  password: string = "";
  confirmPassword: string = "";
  firstName: string | null = null;
  lastName: string | null = null;

  // constructor(data: IRegisterPasswordUserRequest ) {
  //   this.email = data.email;
  //   this.password = data.password;
  //   this.confirmPassword = data.confirmPassword;
  //   this.firstName = data.firstName;
  //   this.lastName = data.lastName;
  // }
}