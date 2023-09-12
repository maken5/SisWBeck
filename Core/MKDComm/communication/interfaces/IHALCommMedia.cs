using System;
using System.Collections.Generic;
using System.Text;

/*  
 * IHALCommObject - Interface Hardware Abstraction Layer Object
 * Interface para métodos de acesso padrões de objetos de comunicação (portas)
 * 
 * Autor: Marcio Ken Iti Doi
 * Data: 30/04/2010
 * 
 * Todos os Direitos Reservados
 */

namespace mkdInfo.communication.interfaces
{
    
    public interface IHALCommMedia
    {
        string getNameComm();
        void setParameter(KeyValuePair<Object, Object>[] parameter);
        void updateParameters();
        bool isOpen();
        Type getType();
        void open();
        void open(KeyValuePair<Object, Object>[] parameter);
        bool close();
        void send(byte[] data);
    }
}
