import java.util.List;

public interface User {

    public List<Room> room = null;
    public UserDB userDB =null;
  
  public void verifyPassword( String passkey);

  public void sendToServer();

  public void receiveFromServer();

  public void exitGame();

}