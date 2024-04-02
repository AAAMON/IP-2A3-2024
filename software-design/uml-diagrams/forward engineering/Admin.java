public class Admin implements User {

  public String username;

  public Integer adminID;

  public Integer clientSocket;

  public void SendAdminComm( AdminCommand adminCommand) {
  }

  public static void main(String[] args) {
    Admin A=new Admin();
  }

  @Override
  public void verifyPassword(String passkey) {

  }

  @Override
  public void sendToServer() {

  }

  @Override
  public void receiveFromServer() {

  }

  @Override
  public void exitGame() {

  }
}